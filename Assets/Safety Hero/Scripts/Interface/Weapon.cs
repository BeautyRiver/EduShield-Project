using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 무기 관련
public interface IBatchable
{
    void Batch();
}

public interface IRotatingable
{
    void Rotate();
}
