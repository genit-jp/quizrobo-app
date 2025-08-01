# IAP ローカライズ生成スクリプト

## 概要

このスクリプトは、アプリ内課金（IAP）のタイトルと説明文を日本語から全言語に翻訳し、fastlane のメタデータファイルを自動生成します。

## 対象プラットフォーム

- **iOS (App Store Connect)**: 41 言語対応
- **Android (Google Play Console)**: 78 言語対応

## 商品情報

- **iOS Product ID**: `jp.genit.quizrobo.noads`
- **Android Product ID**: `jp_genit_quizrobo_noads`
- **商品名**: 広告なし
- **説明**: 広告を完全に削除して、快適なドラムゲーム体験をお楽しみください。

## 必要な環境設定

### 1. 依存パッケージのインストール

```bash
pip install requests python-dotenv
```

### 2. Claude API キーの設定

`.env` ファイルを作成して Claude API キーを設定：

```bash
# .env ファイル
CLAUDE_API_KEY=your_claude_api_key_here
```

または環境変数で設定：

```bash
export CLAUDE_API_KEY=your_claude_api_key_here
```

## 使用方法

### 基本実行

```bash
cd scripts/fastlane/metadata
python translate_iap.py
```

### 生成されるファイル

#### iOS 用ファイル

```
metadata/ios/{locale}/in_app_purchases.txt
```

ファイル形式: `{product_id}|{title}|{description}`

例:

```
jp.genit.quizrobo.noads|No Ads|Remove all advertisements and enjoy a seamless drum game experience.
```

#### Android 用ファイル

```
metadata/android/{locale}/products/jp_genit_quizrobo_noads/title.txt
metadata/android/{locale}/products/jp_genit_quizrobo_noads/description.txt
```

## 対応言語一覧

### iOS 対応言語 (41 言語)

- 日本語 (ja)
- 英語 (en-US, en-GB, en-AU, en-CA)
- 中国語 (zh-Hans, zh-Hant)
- 韓国語 (ko)
- ヨーロッパ言語 (fr-FR, fr-CA, de-DE, es-ES, es-MX, it, pt-BR, pt-PT, ru, nl-NL, sv, da, fi, no, pl, cs, sk, hu, ro, el, hr, ca)
- その他 (ar-SA, he, hi, th, tr, vi, id, ms, uk)

### Android 対応言語 (78 言語)

iOS 対応言語に加えて、より多くの地域言語をサポート

## fastlane でのアップロード

### iOS

```bash
cd scripts/fastlane
fastlane ios upload_iap_metadata
```

### Android

```bash
cd scripts/fastlane
fastlane android upload_iap_metadata
```

## 注意事項

1. **API 制限**: Claude API の制限により、言語数が多いため実行に時間がかかります（約 5-10 分）
2. **エラーハンドリング**: 翻訳エラーが発生した場合、英語版がフォールバックとして使用されます
3. **文字数制限**:
   - タイトル: 30 文字以内推奨
   - 説明: 200 文字以内推奨
4. **レート制限**: API 制限対策で各翻訳間に 1 秒の待機時間を設けています

## カスタマイズ

商品情報を変更したい場合は、`translate_iap.py` の `IAP_DATA` を編集してください：

```python
IAP_DATA = {
    "ios_product_id": "jp.genit.quizrobo.noads",
    "android_product_id": "jp_genit_quizrobo_noads",
    "title": "広告なし",
    "description": "広告を完全に削除して、快適なドラムゲーム体験をお楽しみください。"
}
```

## トラブルシューティング

### Claude API エラー

- API キーが正しく設定されているか確認
- Claude API の利用制限に達していないか確認

### ファイル生成エラー

- ディレクトリの書き込み権限を確認
- 十分なディスク容量があるか確認

### 翻訳品質の改善

- プロンプトを調整して、より自然な翻訳を生成
- 特定言語で問題がある場合は、手動で修正
