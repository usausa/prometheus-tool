namespace WorkDesign;

using System.Reflection;
using System.Windows.Threading;

using SkiaSharp;
using SkiaSharp.Views.Desktop;

public partial class MainWindow
{
    private readonly DispatcherTimer refreshTimer;
    private readonly Random random = new();
    private readonly SKTypeface font;
    private float currentUsage;

    public MainWindow()
    {
        InitializeComponent();

        font = LoadEmbeddedFont("WorkDesign.851Gkktt_005.ttf");

        refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        refreshTimer.Tick += (_, _) =>
        {
#pragma warning disable CA5394
            currentUsage = random.Next(0, 100);
#pragma warning restore CA5394
            Canvas.InvalidateVisual();
        };
        refreshTimer.Start();
    }

    private static SKTypeface LoadEmbeddedFont(string fontResourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        using var fontStream = assembly.GetManifestResourceStream(fontResourceName);
        return SKTypeface.FromStream(fontStream);
    }

    private static SKColor UsageColor(float usage)
    {
        return usage switch
        {
            < 60f => new SKColor(0x4c, 0xaf, 0x50),
            < 85f => new SKColor(0xff, 0x98, 0x00),
            _ => new SKColor(0xf4, 0x43, 0x36)
        };
    }

    private void Canvas_OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var w = e.Info.Width;
        var h = e.Info.Height;
        canvas.Clear(SKColors.Black);

        var color = UsageColor(currentUsage);
        const int pad = 6;
        var borderRect = new SKRect(pad, pad, w - pad, h - pad);
        var cornerRadius = 6f;

        // 角丸の四角形を定義します
        using var roundedRect = new SKRoundRect();
        roundedRect.SetRect(borderRect, cornerRadius, cornerRadius);

        // 1. 外側の影 (box-shadow: 0 0 20px #0f0) の描画
        // CSSのblur半径(px)をSkiaSharpのsigma値に変換するには、一般的に「sigma = 半径 / 3」という近似が使われます。
        using (var outerShadowPaint = new SKPaint())
        {
            outerShadowPaint.IsAntialias = true; // アンチエイリアスを有効にして滑らかに描画します
            outerShadowPaint.Style = SKPaintStyle.Stroke; // 線として描画します
            outerShadowPaint.StrokeWidth = 6f; // ★変更点: 影の幅を5fに調整しました。必要に応じて調整してください。
            outerShadowPaint.Color = color; // 影の色をメインのボーダー色と同じにします

            // 外側の影を作成します。SKBlurStyle.Normal は外側へのぼかしを意味します。
            // 20pxのぼかし半径に対して、sigma値は 20f / 3f となります。
            outerShadowPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 3f);

            // 角丸の四角形に外側の影を描画します
            canvas.DrawRoundRect(roundedRect, outerShadowPaint);
            //canvas.DrawRoundRect(roundedRect, outerShadowPaint);

            //outerShadowPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Inner, 4f);
            //canvas.DrawRoundRect(roundedRect, outerShadowPaint);
        }

        // 3. メインのボーダー (border: 2px solid #0f0) の描画
        // 影の上にメインのボーダーを重ねて描画することで、ボーダーがはっきりと表示されます。
        using (var borderPaint = new SKPaint())
        {
            borderPaint.IsAntialias = true; // アンチエイリアスを有効にします
            borderPaint.Style = SKPaintStyle.Stroke; // 線として描画します
            borderPaint.StrokeWidth = 2f; // CSSのボーダー幅と同じ2pxを設定します
            borderPaint.Color = color; // ボーダーの色を設定します

            // 角丸の四角形にメインのボーダーを描画します
            canvas.DrawRoundRect(roundedRect, borderPaint);
        }

        // CPU使用率バー
        float barX = 20;
        float barY = h - 40;
        float barW = w - 40;
        float barH = 24;
        using (var barPaint = new SKPaint())
        {
            barPaint.IsAntialias = true;
            barPaint.Color = color;

            // 枠
            barPaint.Style = SKPaintStyle.Stroke;
            barPaint.StrokeWidth = 2;
            canvas.DrawRect(barX, barY, barW, barH, barPaint);

            // 中身
            barPaint.Style = SKPaintStyle.Fill;
            canvas.DrawRect(barX, barY, barW * (currentUsage / 100f), barH, barPaint);
        }

        // テキスト描画
        // TODO obsolete
#pragma warning disable CS0618 // Type or member is obsolete
        using (var textPaint = new SKPaint())
        {
            textPaint.IsAntialias = true;
            textPaint.Color = color;
            textPaint.Typeface = font;

            // ラベル
            textPaint.TextSize = 28;
            textPaint.TextAlign = SKTextAlign.Left;
            canvas.DrawText("CPU USAGE", barX, 36, textPaint);

            textPaint.TextAlign = SKTextAlign.Right;
            textPaint.TextSize = 24;
            canvas.DrawText("SERVER", w - barX, 36, textPaint);

            // 使用率数値
            textPaint.TextSize = 72;
            textPaint.TextAlign = SKTextAlign.Right;
            var usageText = $"{(int)currentUsage}%";
            canvas.DrawText(usageText, w - barX, 108, textPaint);
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
