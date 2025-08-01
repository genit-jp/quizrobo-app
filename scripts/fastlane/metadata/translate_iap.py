#!/usr/bin/env python3
# translate_iap.py
# ------------------------------------------------------------
# IAP（アプリ内課金）のローカライズ情報を全言語に翻訳
# usage: python translate_iap.py
# ------------------------------------------------------------
import os, requests, sys, json, time
from dotenv import load_dotenv

# 環境変数を読み込み
load_dotenv()

# Claude API設定
CLAUDE_API_URL = "https://api.anthropic.com/v1/messages"
CLAUDE_API_KEY = os.getenv("CLAUDE_API_KEY")
if not CLAUDE_API_KEY:
    sys.exit("❌  Set CLAUDE_API_KEY in environment variables")
CLAUDE_MODEL = "claude-opus-4-20250514"

# IAP情報（日本語ベース）
IAP_DATA = {
    "ios_product_id": "jp.genit.quizrobo.noads",
    "android_product_id": "jp_genit_quizrobo_noads",
    "title": "広告なし",
    "description": "広告を完全に削除して、快適なドラムゲーム体験をお楽しみください。",
}

# iOS対応言語リスト
SUPPORTED_IOS_LOCALES = [
    "da",
    "de-DE",
    "el",
    "en-AU",
    "en-CA",
    "en-GB",
    "en-US",
    "es-ES",
    "es-MX",
    "fi",
    "fr-CA",
    "fr-FR",
    "id",
    "it",
    "ja",
    "ko",
    "ms",
    "nl-NL",
    "no",
    "pt-BR",
    "pt-PT",
    "ru",
    "sv",
    "th",
    "tr",
    "vi",
    "zh-Hans",
    "zh-Hant",
    "ca",
    "hr",
    "cs",
    "ar-SA",
    "he",
    "hi",
    "hu",
    "pl",
    "ro",
    "sk",
    "uk",
]

# Android（Google Play）対応言語リスト
GOOGLE_PLAY_LOCALES = [
    "af",
    "sq",
    "am",
    "ar",
    "hy-AM",
    "az-AZ",
    "bn-BD",
    "eu-ES",
    "be",
    "bg",
    "my-MM",
    "ca",
    "zh-HK",
    "zh-CN",
    "zh-TW",
    "hr",
    "cs-CZ",
    "da-DK",
    "nl-NL",
    "en-AU",
    "en-CA",
    "en-US",
    "en-GB",
    "en-IN",
    "en-SG",
    "en-ZA",
    "et",
    "fil",
    "fi-FI",
    "fr-CA",
    "fr-FR",
    "gl-ES",
    "ka-GE",
    "de-DE",
    "el-GR",
    "gu",
    "iw-IL",
    "hi-IN",
    "hu-HU",
    "is-IS",
    "id",
    "it-IT",
    "ja-JP",
    "kn-IN",
    "kk",
    "km-KH",
    "ko-KR",
    "ky-KG",
    "lo-LA",
    "lv",
    "lt",
    "mk-MK",
    "ms-MY",
    "ms",
    "ml-IN",
    "mr-IN",
    "mn-MN",
    "ne-NP",
    "no-NO",
    "fa",
    "fa-AE",
    "fa-AF",
    "fa-IR",
    "pl-PL",
    "pt-BR",
    "pt-PT",
    "pa",
    "ro",
    "rm",
    "ru-RU",
    "sr",
    "si-LK",
    "sk",
    "sl",
    "es-419",
    "es-ES",
    "es-US",
    "sw",
    "sv-SE",
    "ta-IN",
    "te-IN",
    "th",
    "tr-TR",
    "uk",
    "ur",
    "vi",
    "zu",
]


def get_claude_lang_code(locale):
    """Claudeが理解しやすいターゲット言語名を返す"""
    mapping = {
        "zh-Hans": "Chinese (Simplified)",
        "zh-Hant": "Chinese (Traditional)",
        "zh-CN": "Chinese (Simplified)",
        "zh-TW": "Chinese (Traditional)",
        "zh-HK": "Chinese (Traditional)",
        "pt-BR": "Portuguese (Brazil)",
        "pt-PT": "Portuguese (Portugal)",
        "en-US": "English",
        "en-GB": "English (UK)",
        "en-AU": "English (Australia)",
        "en-CA": "English (Canada)",
        "en-IN": "English (India)",
        "en-SG": "English (Singapore)",
        "en-ZA": "English (South Africa)",
        "fr-FR": "French",
        "fr-CA": "French (Canada)",
        "es-ES": "Spanish (Spain)",
        "es-MX": "Spanish (Mexico)",
        "es-419": "Spanish (Latin America)",
        "es-US": "Spanish (US)",
        "ja-JP": "Japanese",
        "ko-KR": "Korean",
        "de-DE": "German",
        "it-IT": "Italian",
        "ru-RU": "Russian",
        "fi-FI": "Finnish",
        "sv-SE": "Swedish",
        "nl-NL": "Dutch",
        "da-DK": "Danish",
        "no-NO": "Norwegian",
        "pl-PL": "Polish",
        "cs-CZ": "Czech",
        "tr-TR": "Turkish",
        "ar-SA": "Arabic",
        "iw-IL": "Hebrew",
        "hi-IN": "Hindi",
        "th": "Thai",
        "vi": "Vietnamese",
        "id": "Indonesian",
        "ms": "Malay",
        "ms-MY": "Malay",
    }

    if locale in mapping:
        return mapping[locale]

    # ハイフン区切りの場合、前半部分で判定
    if "-" in locale:
        base = locale.split("-")[0]
        base_mapping = {
            "en": "English",
            "fr": "French",
            "es": "Spanish",
            "de": "German",
            "it": "Italian",
            "pt": "Portuguese",
            "ru": "Russian",
            "ko": "Korean",
            "zh": "Chinese",
            "ja": "Japanese",
            "ar": "Arabic",
            "hi": "Hindi",
            "tr": "Turkish",
            "pl": "Polish",
            "nl": "Dutch",
            "sv": "Swedish",
            "da": "Danish",
            "fi": "Finnish",
            "no": "Norwegian",
            "cs": "Czech",
            "sk": "Slovak",
            "hu": "Hungarian",
            "ro": "Romanian",
            "el": "Greek",
            "bg": "Bulgarian",
            "hr": "Croatian",
            "sl": "Slovenian",
            "et": "Estonian",
            "lv": "Latvian",
            "lt": "Lithuanian",
            "uk": "Ukrainian",
            "th": "Thai",
            "vi": "Vietnamese",
            "id": "Indonesian",
            "ms": "Malay",
        }
        if base in base_mapping:
            return base_mapping[base]

    return locale


def translate_claude(title, description, target_lang):
    """Claude APIを使って翻訳"""
    prompt = f"""以下の日本語のアプリ内課金商品の情報を、{target_lang}に自然なアプリストア用表現で翻訳してください。

タイトル: {title}
説明: {description}

出力はJSON形式で以下のようにしてください:
{{
  "title": "（翻訳されたタイトル）",
  "description": "（翻訳された説明）"
}}

注意事項:
- タイトルは簡潔で分かりやすく（30文字以内推奨）
- 説明は商品の価値を明確に伝える内容で（200文字以内推奨）
- その言語圏のユーザーに自然な表現を使用
"""

    headers = {
        "x-api-key": CLAUDE_API_KEY,
        "anthropic-version": "2023-06-01",
        "content-type": "application/json",
    }

    data = {
        "model": CLAUDE_MODEL,
        "max_tokens": 1024,
        "messages": [{"role": "user", "content": prompt}],
    }

    for attempt in range(3):  # リトライ最大3回
        try:
            r = requests.post(
                CLAUDE_API_URL, headers=headers, data=json.dumps(data), timeout=60
            )
            r.raise_for_status()
            content = r.json()["content"][0]["text"]

            # JSON部分だけ抽出
            json_start = content.find("{")
            json_end = content.rfind("}") + 1
            json_str = content[json_start:json_end]
            result = json.loads(json_str)

            return result["title"], result["description"]

        except Exception as e:
            print(f"Claude API error (attempt {attempt + 1}): {e}")
            if attempt < 2:
                time.sleep(2)
            else:
                raise RuntimeError(f"Claude API failed after 3 retries: {e}")


def write_file(path: str, content: str):
    """ファイルを書き込み"""
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, "w", encoding="utf-8") as f:
        f.write(content)


def create_ios_iap_files():
    """iOS用のIAPファイルを生成"""
    print("🍎 iOS IAP files generation started...")

    for locale in SUPPORTED_IOS_LOCALES:
        ios_dir = f"ios/{locale}"
        os.makedirs(ios_dir, exist_ok=True)

        if locale == "ja":
            # 日本語はベース言語なのでそのまま使用
            print(f"🌐  {locale} - Japanese (base language)")
            iap_content = f"{IAP_DATA['ios_product_id']}|{IAP_DATA['title']}|{IAP_DATA['description']}"
            write_file(os.path.join(ios_dir, "in_app_purchases.txt"), iap_content)
            continue

        target_lang = get_claude_lang_code(locale)
        print(f"🌐  {locale} - {target_lang}")

        try:
            title, description = translate_claude(
                IAP_DATA["title"], IAP_DATA["description"], target_lang
            )

            iap_content = f"{IAP_DATA['ios_product_id']}|{title}|{description}"
            write_file(os.path.join(ios_dir, "in_app_purchases.txt"), iap_content)

            # API制限対策で少し待機
            time.sleep(1)

        except Exception as e:
            print(f"❌ Error translating {locale}: {e}")
            # エラーの場合は英語版をフォールバック
            fallback_content = f"{IAP_DATA['ios_product_id']}|No Ads|Remove all advertisements and enjoy a seamless drum game experience."
            write_file(os.path.join(ios_dir, "in_app_purchases.txt"), fallback_content)


def create_android_iap_files():
    """Android用のIAPファイルを生成"""
    print("🤖 Android IAP files generation started...")

    for locale in GOOGLE_PLAY_LOCALES:
        android_dir = f"android/{locale}/products/{IAP_DATA['android_product_id']}"
        os.makedirs(android_dir, exist_ok=True)

        if locale in ["ja", "ja-JP"]:
            # 日本語はベース言語なのでそのまま使用
            print(f"🌐  {locale} - Japanese (base language)")
            write_file(os.path.join(android_dir, "title.txt"), IAP_DATA["title"])
            write_file(
                os.path.join(android_dir, "description.txt"), IAP_DATA["description"]
            )
            continue

        target_lang = get_claude_lang_code(locale)
        print(f"🌐  {locale} - {target_lang}")

        try:
            title, description = translate_claude(
                IAP_DATA["title"], IAP_DATA["description"], target_lang
            )

            write_file(os.path.join(android_dir, "title.txt"), title)
            write_file(os.path.join(android_dir, "description.txt"), description)

            # API制限対策で少し待機
            time.sleep(1)

        except Exception as e:
            print(f"❌ Error translating {locale}: {e}")
            # エラーの場合は英語版をフォールバック
            write_file(os.path.join(android_dir, "title.txt"), "No Ads")
            write_file(
                os.path.join(android_dir, "description.txt"),
                "Remove all advertisements and enjoy a seamless drum game experience.",
            )


def main():
    """メイン処理"""
    print("🚀 Starting IAP localization generation...")
    print(f"📱 iOS Product ID: {IAP_DATA['ios_product_id']}")
    print(f"🤖 Android Product ID: {IAP_DATA['android_product_id']}")
    print(f"📝 Base Title: {IAP_DATA['title']}")
    print(f"📝 Base Description: {IAP_DATA['description']}")
    print()

    # iOS用ファイル生成
    create_ios_iap_files()
    print()

    # Android用ファイル生成
    create_android_iap_files()
    print()

    print("✅ All IAP localization files generated successfully!")
    print(f"📊 Generated files for {len(SUPPORTED_IOS_LOCALES)} iOS locales")
    print(f"📊 Generated files for {len(GOOGLE_PLAY_LOCALES)} Android locales")


if __name__ == "__main__":
    main()
