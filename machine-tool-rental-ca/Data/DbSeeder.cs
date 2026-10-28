using machine_tool_rental_ca.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace machine_tool_rental_ca.Data;

/// <summary>
/// LaravelのSeeder相当。起動時にマイグレーションを適用し、固定の管理者とモック掲載の機械6件を作成する。
/// 何度実行しても重複しない（存在しなければ作る）。
/// </summary>
public static class DbSeeder
{
    // デモ用の固定アカウント
    public const string AdminUserName = "admin";
    public const string AdminPassword = "demo-password";

    public static async Task MigrateAndSeedAsync(IServiceProvider services)
    {
        var factory = services.GetRequiredService<IDbContextFactory<AppDbContext>>();
        var hasher = services.GetRequiredService<IPasswordHasher<AdminUser>>();

        await using var db = await factory.CreateDbContextAsync();
        await db.Database.MigrateAsync();

        if (!await db.AdminUsers.AnyAsync(u => u.UserName == AdminUserName))
        {
            var admin = new AdminUser { UserName = AdminUserName };
            admin.PasswordHash = hasher.HashPassword(admin, AdminPassword);
            db.AdminUsers.Add(admin);
        }

        if (!await db.RentalMachines.AnyAsync())
        {
            db.RentalMachines.AddRange(
                new RentalMachine
                {
                    Name = "汎用旋盤 TL-460",
                    Category = "旋盤",
                    Manufacturer = "東和精機",
                    ManufactureYear = 2018,
                    Status = MachineStatus.Available,
                    MonthlyPrice = 120000,
                    MinRentalPeriod = "1週間〜",
                    InstallArea = "関東一円に搬入可",
                    NextAvailable = "即日〜",
                    Features = "（サンプル）汎用性の高い旋盤です。貸出前に精度検査を行います。",
                    Accessories = "操作マニュアル（日本語）",
                    Specs = Specs(("振り", "460 mm"), ("心間", "1,000 mm"), ("主軸回転数", "40–2,000 min⁻¹"))
                },
                new RentalMachine
                {
                    Name = "立形MC VX-500",
                    Category = "マシニングセンタ",
                    Manufacturer = "北斗工機",
                    ManufactureYear = 2020,
                    Status = MachineStatus.Available,
                    MonthlyPrice = 480000,
                    MinRentalPeriod = "1か月〜",
                    InstallArea = "関東一円に搬入可",
                    NextAvailable = "即日〜",
                    Features =
                        "小型部品から中型部品までの切削に向く、汎用性の高い立形マシニングセンタです。貸出前に主軸精度とバックラッシを検査し、消耗部品を交換してお渡しします。\n" +
                        "試作や短納期の追加ロットなど、設備を持たずに加工量を増やしたい場面でご利用いただけます。",
                    Accessories = "NC装置 操作マニュアル（日本語）\n基本工具セット（有償）\n切削油タンク・チップコンベア",
                    Specs = Specs(
                        ("X軸ストローク", "510 mm"),
                        ("Y軸ストローク", "400 mm"),
                        ("Z軸ストローク", "350 mm"),
                        ("テーブル寸法", "600 × 400 mm"),
                        ("主軸回転数", "12,000 min⁻¹"),
                        ("主軸テーパ", "BT40"),
                        ("工具収納本数（ATC）", "24本"),
                        ("NC装置", "FANUC 0i-MF"),
                        ("電源", "三相200V / 15 kVA"),
                        ("機械質量", "約 3,800 kg"))
                },
                new RentalMachine
                {
                    Name = "ニー形フライス盤 KM-3",
                    Category = "フライス盤",
                    Manufacturer = "大和製作所",
                    ManufactureYear = 2016,
                    Status = MachineStatus.Reserved,
                    MonthlyPrice = 90000,
                    MinRentalPeriod = "1週間〜",
                    InstallArea = "関東一円に搬入可",
                    NextAvailable = "要相談",
                    Features = "（サンプル）段取り替えの多い現場向けのニー形フライス盤です。",
                    Accessories = "操作マニュアル（日本語）",
                    Specs = Specs(("テーブル", "1,200×280 mm"), ("主軸回転数", "60–3,600 min⁻¹"), ("主軸テーパ", "NT40"))
                },
                new RentalMachine
                {
                    Name = "平面研削盤 SG-818",
                    Category = "研削盤",
                    Manufacturer = "東和精機",
                    ManufactureYear = 2019,
                    Status = MachineStatus.Available,
                    MonthlyPrice = 100000,
                    MinRentalPeriod = "1週間〜",
                    InstallArea = "関東一円に搬入可",
                    NextAvailable = "即日〜",
                    Features = "（サンプル）平面仕上げ用の研削盤です。",
                    Accessories = "操作マニュアル（日本語）",
                    Specs = Specs(("テーブル", "200×450 mm"), ("砥石径", "205 mm"), ("送り", "手動/自動"))
                },
                new RentalMachine
                {
                    Name = "半自動溶接機 WM-350",
                    Category = "溶接機",
                    Manufacturer = "日進電機",
                    ManufactureYear = 2021,
                    Status = MachineStatus.Available,
                    MonthlyPrice = 35000,
                    MinRentalPeriod = "1週間〜",
                    InstallArea = "関東一円に搬入可",
                    NextAvailable = "即日〜",
                    Features = "（サンプル）板金・構造物の溶接に使用できる半自動溶接機です。",
                    Accessories = "操作マニュアル（日本語）",
                    Specs = Specs(("定格電流", "350 A"), ("電源", "三相200V"), ("ワイヤ径", "0.8–1.2 mm"))
                },
                new RentalMachine
                {
                    Name = "横形MC HX-400",
                    Category = "マシニングセンタ",
                    Manufacturer = "北斗工機",
                    ManufactureYear = 2017,
                    Status = MachineStatus.Reserved,
                    MonthlyPrice = 650000,
                    MinRentalPeriod = "1か月〜",
                    InstallArea = "関東一円に搬入可",
                    NextAvailable = "要相談",
                    Features = "（サンプル）パレットチェンジャ付きの横形マシニングセンタです。",
                    Accessories = "NC装置 操作マニュアル（日本語）",
                    Specs = Specs(("パレット", "400×400 mm"), ("主軸回転数", "10,000 min⁻¹"), ("ATC", "40本"))
                });
        }

        await db.SaveChangesAsync();
    }

    private static List<MachineSpec> Specs(params (string Name, string Value)[] rows) =>
        rows.Select((r, i) => new MachineSpec { Name = r.Name, Value = r.Value, SortOrder = i }).ToList();
}
