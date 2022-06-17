using HashidsNet;

namespace XTrakr.Common;
public static class IdEncoder
{
    private readonly static string _salt = ")gceN/CyxGK@ctS8IMa>_B')?t9MW]~/x^t]1]FOZH<uoS{`7u;TLQ@:mvW7~:(";
    private readonly static IHashids _hasher;

    static IdEncoder() => _hasher = new Hashids(_salt, 20);

    public static string EncodeId(int id) => _hasher.Encode(id);

    public static int DecodeId(string hash)
    {
        try
        {
            return _hasher.Decode(hash)?.FirstOrDefault() ?? 0;
        }
        catch
        {
            return 0;
        }
    }
}
