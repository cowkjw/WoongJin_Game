using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;

public enum ProblemType
{
    A, // 최대 공약수 최소 공배수
    B // 방정식
}
public class ProblemConverter : MonoBehaviour
{
    public string ProblemConvert(string problem,ProblemType problemType)
    {
        string numPattern = @"\d+";
        List<string> numList = new List<string>();
        MatchCollection findNum = Regex.Matches(problem, numPattern);
        foreach (Match match in findNum)
        { 
            numList.Add(match.Value);
        }
        string ret = string.Empty;
        switch (problemType)
        {
            case ProblemType.A:
                ret = $"\\left({numList[0]}, {numList[1]}\\right)\\rightarrow ?";
                break;
                case ProblemType.B:
               // ret = $"{numList[0]} +  $x$  = {numList[1]}  \\rightarrow   $x$  =  ? ";
                ret = new string(problem.Where(c => c != '~').ToArray());
                break;
        }

        return ret;
    }
}
