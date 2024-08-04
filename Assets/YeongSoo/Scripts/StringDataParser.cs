/// <summary>
/// string �����͸� �Ľ��ϴ� Ŀ���� Ŭ���� �Դϴ�
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
