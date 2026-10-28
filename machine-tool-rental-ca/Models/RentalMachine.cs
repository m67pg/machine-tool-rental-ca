using System.ComponentModel.DataAnnotations;

namespace machine_tool_rental_ca.Models;

public enum MachineStatus
{
    Available = 0, // 空きあり
    Reserved = 1   // 予約中
}

public class RentalMachine
{
    public int Id { get; set; }

    [Required(ErrorMessage = "機械名は必須です")]
    [StringLength(200, ErrorMessage = "機械名は200文字以内で入力してください")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "カテゴリは必須です")]
    [StringLength(100, ErrorMessage = "カテゴリは100文字以内で入力してください")]
    public string Category { get; set; } = "";

    [StringLength(100)]
    public string? Manufacturer { get; set; }

    [Range(1900, 2100, ErrorMessage = "製造年は1900〜2100で入力してください")]
    public int? ManufactureYear { get; set; }

    public MachineStatus Status { get; set; } = MachineStatus.Available;

    [Range(0, 100_000_000, ErrorMessage = "月額は0以上で入力してください")]
    public decimal MonthlyPrice { get; set; }

    [StringLength(50)]
    public string? MinRentalPeriod { get; set; }   // 例: 1か月〜

    [StringLength(100)]
    public string? InstallArea { get; set; }       // 例: 関東一円に搬入可

    [StringLength(50)]
    public string? NextAvailable { get; set; }     // 例: 即日〜

    [StringLength(4000, ErrorMessage = "機械の特徴は4000文字以内で入力してください")]
    public string? Features { get; set; }          // 機械の特徴（改行ごとに段落）

    [StringLength(2000, ErrorMessage = "付属品は2000文字以内で入力してください")]
    public string? Accessories { get; set; }       // 付属品（1行1項目）

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<MachineSpec> Specs { get; set; } = [];
}

/// <summary>仕様表の1行（例: X軸ストローク / 510 mm）。機械ごとに行数・項目が違うため別テーブル。</summary>
public class MachineSpec
{
    public int Id { get; set; }
    public int RentalMachineId { get; set; }
    public RentalMachine? RentalMachine { get; set; }

    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
    public int SortOrder { get; set; }
}

public static class RentalMachineExtensions
{
    public static string Label(this MachineStatus status) => status switch
    {
        MachineStatus.Available => "空きあり",
        MachineStatus.Reserved => "予約中",
        _ => ""
    };

    public static string CssClass(this MachineStatus status) => status switch
    {
        MachineStatus.Available => "status-ok",
        MachineStatus.Reserved => "status-wait",
        _ => ""
    };

    /// <summary>「北斗工機 / 2020年製」</summary>
    public static string MakerLine(this RentalMachine m) =>
        string.Join(" / ", new[]
        {
            m.Manufacturer,
            m.ManufactureYear is { } y ? $"{y}年製" : null
        }.Where(s => !string.IsNullOrWhiteSpace(s)));

    /// <summary>改行区切りのテキストを空行なしの配列にする</summary>
    public static string[] ToLines(this string? text) =>
        (text ?? "").Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
