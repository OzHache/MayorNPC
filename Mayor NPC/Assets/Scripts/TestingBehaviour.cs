using UnityEngine;
using System.Collections;

public class TestingBehaviour
{

    private protected float _aPrivateFloat = 2;
    

}
public class subTestingBehaviour : TestingBehaviour
{
    public float aPublicFloat { get { return base._aPrivateFloat; }
        set { base._aPrivateFloat = value; } }

    public void ChangeMyBasePrivateFloat (float constructorFloat)
    {
        base._aPrivateFloat = 1f;
    }
}

public class testing
{
    public subTestingBehaviour STB = new subTestingBehaviour();
    
    void DoAThing()
    {
        Debug.Log(STB.aPublicFloat);
    }
}
