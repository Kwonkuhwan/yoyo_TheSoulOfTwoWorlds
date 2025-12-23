using UnityEngine;
using TMPro;

public class TouchKeypad : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI numberText;

    string blank = "_"; //입력되지 않은 키패드

    int currentIndex;
    int?[] numbers = new int?[4]; //숫자가 저장될 배열

    private void Awake()
    {
        ResetNumber();
    }

    /// <summary>
    /// 키패드 입력값을 순서대로 저장
    /// </summary>
    public void SetNumber(int getNumber)
    {
        if (currentIndex > 3) return; //4자리까지만 입력 가능

        numbers[currentIndex] = getNumber;
        currentIndex++;

        TextRenew(); //UI 갱신
    }

    /// <summary>
    /// 키패드 초기화 (모든 입력값 제거)
    /// </summary>
    public void ResetNumber()
    {
        currentIndex = 0;

        //입력값을 모두 null로 초기화 → "입력 없음" 상태 표현
        for (int i = 0; i < 4; i++)
        {
            numbers[i] = null;
        }

        TextRenew();
    }

    /// <summary>
    /// UI 텍스트 갱신 (입력값 or 빈칸 출력)
    /// </summary>
    void TextRenew()
    {
        string[] numberParts = new string[numbers.Length];

        for (int i = 0; i < numbers.Length; i++)
        {
            //값이 있으면 ToString(), 없으면 blank("_") 표시
            numberParts[i] = numbers[i]?.ToString() ?? blank;
        }

        //원본
        //numberText.text = string.Join(" ",   //뒤에 추가될 문자열 사이에 들어갈 텍스트. 여기선 공백(띄어쓰기)
        //    numbers[0]?.ToString() ?? blank, //numbers[0]가 null일때 => blank / null이 아닐때 => numbers[0].ToString()
        //    numbers[1]?.ToString() ?? blank,
        //    numbers[2]?.ToString() ?? blank,
        //    numbers[3]?.ToString() ?? blank
        //    );

        //4자리 문자열을 공백으로 구분해 출력 (예: "1 _ 3 _")
        numberText.text = string.Join(" ", numberParts);
    }


    /// <summary>
    /// 입력한 비밀번호가 맞는지 확인
    /// </summary>
    public void SubmitNumber()
    {
        
    }
}
