using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System.Threading.Tasks; 
using System.Collections.Generic; 

public class FirebaseManager : MonoBehaviour
{
    // 1. 싱글톤 패턴 구현
    private static FirebaseManager instance = null;
    public static FirebaseManager Instance
    {
        get
        {
            if (instance == null) // 인스턴스가 없다면
            {
                GameObject go = new GameObject("FirebaseManager");
                instance = go.AddComponent<FirebaseManager>();                
            }
            return instance;
        }
    }

    // 2. Firebase 핵심 변수들
    public FirebaseAuth auth;
    public FirebaseFirestore db; // 데이터베이스(Firestore) 변수 추가
    public FirebaseUser user;

    // 3. 게임 시작 시 초기화 (MonoBehaviour의 Awake 활용)
    void Awake()
    {
        // 이미 다른 인스턴스가 있다면 이 오브젝트는 파괴
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        // 씬을 전환해도 이 매니저 오브젝트는 파괴되지 않도록 설정!
        DontDestroyOnLoad(this.gameObject);

        // Firebase 종속성 확인 및 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }

    void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance; // Firestore 인스턴스 초기화
        auth.StateChanged += OnAuthStateChanged; // 로그인 상태 변경 감지 시작
        OnAuthStateChanged(this, null); // 초기 상태 확인
    }

    // 4. 로그인 상태 변경 감지 (네 코드의 장점!)
    void OnAuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("로그아웃 되었습니다.");
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log($"로그인 되었습니다: {user.Email} (UID: {user.UserId})");
            }
        }
    }

    // 5. 회원가입 및 로그인 함수 (async/await로 가독성 UP)
    public async Task<string> CreateAccount(string email, string password)
    {
        try
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return "이메일과 비밀번호를 모두 입력해주세요.";
            }
            await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            Debug.Log("회원가입 성공!");
            return null;
        }
        catch (FirebaseException e)
        {
            AuthError errorCAode = (AuthError)e.ErrorCode;
            string message = "회원가입에 실패했습니다."; // Default

            switch (errorCAode)
            {
                case AuthError.EmailAlreadyInUse:
                    message = "이미 사용 중인 이메일입니다.";
                    break;
                case AuthError.InvalidEmail:
                    message = "유효하지 않은 이메일 형식입니다.";
                    break;
                case AuthError.WeakPassword:
                    message = "비밀번호는 6자리 이상이어야 합니다.";
                    break;
            }
            Debug.LogError($"회원가입 실패: {message} (코드: {errorCAode}");
            return message;
        }
    }

    public async Task<string> Login(string email, string password)
    {
        try
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return "이메일과 비밀번호를 모두 입력해주세요.";
            }

            await auth.SignInWithEmailAndPasswordAsync(email, password);
            Debug.Log("로그인 성공!");
            var userData = await LoadUserData();
            if (userData != null)
            {
                DataManager.instance.InitializedData(userData);
            }
            else
            {
                DataManager.instance.InitializedData(null);
                await DataManager.instance.SaveGameData(); // 기본 데이터 저장
                Debug.Log("기본 유저 데이터가 생성되었습니다.");
            }
            return null; // 성공 시 null 반환
        }
        catch (FirebaseException e)
        {
            AuthError errorCode = (AuthError)e.ErrorCode;
            string message = "로그인에 실패했습니다.";

            switch (errorCode)
            {
                case AuthError.WrongPassword:
                    message = "비밀번호가 일치하지 않습니다.";
                    break;
                case AuthError.UserNotFound:
                    message = "존재하지 않는 계정입니다.";
                    break;
                case AuthError.InvalidEmail:
                    message = "유효하지 않은 이메일 형식입니다.";
                    break;
            }
            Debug.LogError($"로그인 실패: {message} (코드: {errorCode})");
            return message; // 구체적인 에러 메시지 반환
        }
    }


    public void Logout()
    {
        auth.SignOut();
    }


    // 6. [가장 중요!] 데이터 저장 및 불러오기 함수
    public async Task SaveUserData(Dictionary<string, object> userData)
    {
        if (user == null)
        {
            Debug.LogError("로그인 상태가 아닙니다. 데이터를 저장할 수 없습니다.");
            return;
        }

        // 'users' 컬렉션 안에 현재 유저의 UID로 된 문서를 가져옴
        DocumentReference docRef = db.Collection("users").Document(user.UserId);

        // SetOptions.MergeAll을 사용하면 userData에 있는 필드만 덮어쓰거나 추가해줌. (아주 중요!)
        await docRef.SetAsync(userData, SetOptions.MergeAll);
        Debug.Log("유저 데이터 저장 완료!");
    }


    public async Task<Dictionary<string, object>> LoadUserData()
    {
        if (user == null)
        {
            Debug.LogError("로그인 상태가 아닙니다. 데이터를 불러올 수 없습니다.");
            return null;
        }

        DocumentReference docRef = db.Collection("users").Document(user.UserId);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            Debug.Log("유저 데이터 불러오기 성공!");
            return snapshot.ToDictionary(); // 문서의 모든 데이터를 Dictionary로 변환해서 반환
        }
        else
        {
            Debug.LogWarning("저장된 데이터가 없습니다. 새로운 유저일 수 있습니다.");
            return null;
        }
    }
}