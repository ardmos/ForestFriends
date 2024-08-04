/// <summary>
/// string 데이터를 파싱하는 커스텀 클래스 입니다
/// </summary>
public static class StringDataParser
{
    public static float ParseToFloat(string str)
    {
        return float.TryParse(str, out float parseResult) ? parseResult : 0f;
    }

    public static int ParseToInt(string str)
    {
        return int.TryParse(str, out int parseResult) ? parseResult : 0;
    }
}
