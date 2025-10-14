using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore; // Firestore 데이터베이스 사용을 위해 추가!
using System.Threading.Tasks; // async/await를 위해 필수!
using System.Collections.Generic; // Dictionary를 사용하기 위해 추가!

// 유니티 오브젝트의 생명주기를 사용하면서, 싱글톤으로 존재하도록 MonoBehaviour 상속
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
    public async Task<bool> CreateAccount(string email, string password)
    {
        try
        {
            await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            Debug.Log("회원가입 성공!");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"회원가입 실패: {e.Message}");
            return false;
        }
    }

    public async Task<bool> Login(string email, string password)
    {
        try
        {
            await auth.SignInWithEmailAndPasswordAsync(email, password);
            Debug.Log("로그인 성공!");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"로그인 실패: {e.Message}");
            return false;
        }
    }

    public void Logout()
    {
        auth.SignOut();
    }


    // 6. [가장 중요!] 데이터 저장 및 불러오기 함수
    public async Task SaveUserData(int gold, bool isCharacterUnlocked)
    {
        if (user == null)
        {
            Debug.LogError("로그인 상태가 아닙니다. 데이터를 저장할 수 없습니다.");
            return;
        }

        // Firestore는 Dictionary 형태로 데이터를 저장해.
        var userData = new Dictionary<string, object>
        {
            { "gold", gold },
            { "character_unlocked", isCharacterUnlocked }
            // 나중에 여기에 다이아, 스테이지 레벨 등 계속 추가하면 돼!
        };

        // 'users'라는 컬렉션(폴더) 안에, 현재 유저의 UID로 된 문서(파일)를 만들고 데이터를 덮어쓰기
        DocumentReference docRef = db.Collection("users").Document(user.UserId);
        // SetOptions.MergeAll을 사용하면 기존 다른 데이터는 놔두고 gold, character_unlocked 필드만 업데이트/추가해줘. (아주 중요!)
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