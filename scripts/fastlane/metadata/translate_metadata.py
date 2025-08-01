#!/usr/bin/env python3
# translate_metadata.py
# ------------------------------------------------------------
# CSV → DeepL → metadata へ多言語ファイル出力
# usage: python translate_metadata.py --csv store_metadata_template.csv
# ------------------------------------------------------------
import csv, os, argparse, requests, sys, json, time
from dotenv import load_dotenv

# 環境変数を読み込み
load_dotenv()

# Claude API設定
CLAUDE_API_URL = "https://api.anthropic.com/v1/messages"
CLAUDE_API_KEY = os.getenv("CLAUDE_API_KEY")
if not CLAUDE_API_KEY:
    sys.exit("❌  Set CLAUDE_API_KEY in environment variables")
CLAUDE_MODEL = "claude-opus-4-20250514"
BATCH_SIZE = 10

# App Store対応言語（DeepL対応言語のみ）
# フォルダ名: (DeepL言語コード, 言語名)
SUPPORTED_LOCALES = {
    "ja": ("JA", "Japanese"),  # ベース言語（翻訳しない）
    "en-US": ("EN", "English"),
    "en-GB": ("EN", "English (UK)"),
    "fr-FR": ("FR", "French"),
    "de-DE": ("DE", "German"),
    "es-ES": ("ES", "Spanish"),
    "it": ("IT", "Italian"),
    "pt-BR": ("PT", "Portuguese (Brazil)"),
    "pt-PT": ("PT", "Portuguese (Portugal)"),
    "ru": ("RU", "Russian"),
    "ko": ("KO", "Korean"),
    "zh-Hans": ("ZH", "Chinese (Simplified)"),
    "zh-Hant": ("ZH-HANT", "Chinese (Traditional)"),
    "nl-NL": ("NL", "Dutch"),
    "sv": ("SV", "Swedish"),
    "da": ("DA", "Danish"),
    "fi": ("FI", "Finnish"),
    "no": ("NB", "Norwegian"),
    "pl": ("PL", "Polish"),
    "tr": ("TR", "Turkish"),
    "ar-SA": ("AR", "Arabic"),
    "uk": ("UK", "Ukrainian"),
    "cs": ("CS", "Czech"),
    "sk": ("SK", "Slovak"),
    "hu": ("HU", "Hungarian"),
    "ro": ("RO", "Romanian"),
    "el": ("EL", "Greek"),
    "bg": ("BG", "Bulgarian"),
    "et": ("ET", "Estonian"),
    "lv": ("LV", "Latvian"),
    "lt": ("LT", "Lithuanian"),
    "sl": ("SL", "Slovenian"),
    "id": ("ID", "Indonesian"),
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
# テスト用短縮リスト例（必要なときだけコメント解除して使う）
# SUPPORTED_IOS_LOCALES = ["en-US", "fr-FR", "de-DE", "ja"]

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
# テスト用短縮リスト例（必要なときだけコメント解除して使う）
# GOOGLE_PLAY_LOCALES = ["en-US", "fr-FR", "de-DE", "ja-JP"]

# ---- fastlane キー ↔ ファイルパスマッピング --------------------
IOS_MAP = {
    "ios_name": "name.txt",
    "ios_subtitle": "subtitle.txt",
    "ios_description": "description.txt",
    "ios_keywords": "keywords.txt",
    "ios_promotional_text": "promotional_text.txt",
    "ios_release_notes": "release_notes.txt",
    "ios_marketing_url": "marketing_url.txt",
    "ios_support_url": "support_url.txt",
    "ios_privacy_url": "privacy_url.txt",
}

ANDROID_MAP = {
    "android_title": "title.txt",
    "android_short_description": "short_description.txt",
    "android_full_description": "full_description.txt",
    "android_video_url": "video.txt",
    # "What's New" は changelogs ディレクトリ
    "android_whats_new": "changelogs/default.txt",
}


def build_claude_prompt(texts, target_lang, max_lengths=None):
    prompt = f"""以下の日本語の文を、{target_lang}に自然なアプリストア用表現で翻訳してください。\n"""
    if max_lengths:
        prompt += "各翻訳文は、指定された最大文字数以内に収めてください。\n"
    prompt += "出力はJSON形式で、各文のインデックスをキー、翻訳文を値としてください。\n\n# 日本語文リスト\n"
    for idx, text in enumerate(texts):
        if max_lengths:
            prompt += f"{idx}: {text} (最大{max_lengths[idx]}文字)\n"
        else:
            prompt += f"{idx}: {text}\n"
    prompt += """\n# 出力例\n{
  \"0\": \"（翻訳文1）\",
  \"1\": \"（翻訳文2）\"
}\n"""
    return prompt


def translate_claude(texts, target_lang, max_lengths=None):
    prompt = build_claude_prompt(texts, target_lang, max_lengths)
    headers = {
        "x-api-key": CLAUDE_API_KEY,
        "anthropic-version": "2023-06-01",
        "content-type": "application/json",
    }
    data = {
        "model": CLAUDE_MODEL,
        "max_tokens": 2048,
        "messages": [{"role": "user", "content": prompt}],
    }
    for _ in range(3):  # リトライ最大3回
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
            translations = [result[str(i)] for i in range(len(texts))]
            # 文字数制限を超えていた場合は自動でカット
            if max_lengths:
                translations = [
                    (
                        t[: max_lengths[i]]
                        if max_lengths[i] is not None and len(t) > max_lengths[i]
                        else t
                    )
                    for i, t in enumerate(translations)
                ]
            return translations
        except Exception as e:
            print(f"Claude API error: {e}. Retrying...")
            time.sleep(2)
    raise RuntimeError("Claude API failed after 3 retries.")


def translate(text: str, target: str, max_length: int = None) -> str:
    # 1件だけ翻訳したい場合もバッチ化
    if max_length:
        return translate_claude([text], target, [max_length])[0]
    else:
        return translate_claude([text], target)[0]


def write_file(path: str, content: str):
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, "w", encoding="utf-8") as f:
        f.write(content)


def get_claude_lang_code(locale):
    # Claudeが理解しやすいターゲット言語名を返す
    # 例外対応
    mapping = {
        "zh-Hans": "zh-Hans",  # 簡体字中国語
        "zh-Hant": "zh-Hant",  # 繁体字中国語
        "pt-BR": "pt-BR",
        "pt-PT": "pt-PT",
        "en-US": "en",
        "en-GB": "en",
        "fr-FR": "fr",
        "fr-CA": "fr",
        "es-ES": "es",
        "es-MX": "es",
        "ja-JP": "ja",
        "ko-KR": "ko",
        "de-DE": "de",
        "it-IT": "it",
        "ru-RU": "ru",
        "fi-FI": "fi",
        "sv-SE": "sv",
        "nl-NL": "nl",
        "da-DK": "da",
        "no-NO": "no",
        "pl-PL": "pl",
        "cs-CZ": "cs",
        "sk": "sk",
        "tr-TR": "tr",
        "ar-SA": "ar",
        "id": "id",
        "ms": "ms",
        "th": "th",
        "vi": "vi",
        # 必要に応じて追加
    }
    # 例外がなければ、ハイフン区切りの前半 or 2文字部分を返す
    if locale in mapping:
        return mapping[locale]
    if "-" in locale:
        return locale.split("-")[0]
    return locale


def main(csv_path: str):
    # CSV → dict
    base = {}
    max_lengths = {}
    with open(csv_path, newline="", encoding="utf-8") as f:
        reader = csv.DictReader(f)
        for row in reader:
            base[row["key"]] = row["value"]
            if "max_length" in row and row["max_length"].strip():
                try:
                    max_lengths[row["key"]] = int(row["max_length"])
                except Exception:
                    pass

    # iOS用ディレクトリを全て作成
    for locale in SUPPORTED_IOS_LOCALES:
        ios_dir = f"metadata/ios/{locale}"
        os.makedirs(ios_dir, exist_ok=True)

    # Android用ディレクトリを全て作成
    for locale in GOOGLE_PLAY_LOCALES:
        android_dir = f"metadata/android/{locale}"
        os.makedirs(android_dir, exist_ok=True)

    # iOS全言語でバッチ翻訳
    for locale in SUPPORTED_IOS_LOCALES:
        ios_dir = f"metadata/ios/{locale}"
        keys = [k for k in IOS_MAP if k in base]
        texts = [base[k] for k in keys]
        maxs = [max_lengths.get(k) for k in keys]
        if locale == "ja":
            print(f"🌐  {locale} - 日本語 (スキップ)")
            for k, fname in IOS_MAP.items():
                if k not in base:
                    continue
                text = base[k]
                write_file(os.path.join(ios_dir, fname), text)
            continue
        target_lang = get_claude_lang_code(locale)
        print(f"🌐  {locale} - Claudeターゲット: {target_lang}")
        translations = translate_claude(texts, target_lang, maxs)
        for (k, fname), trans in zip([(k, IOS_MAP[k]) for k in keys], translations):
            write_file(os.path.join(ios_dir, fname), trans)

    # Android全言語でバッチ翻訳
    for locale in GOOGLE_PLAY_LOCALES:
        and_dir = f"metadata/android/{locale}"
        keys = [k for k in ANDROID_MAP if k in base]
        texts = [base[k] for k in keys]
        maxs = [max_lengths.get(k) for k in keys]
        if locale in ["ja", "ja-JP"]:
            print(f"🌐  {locale} - 日本語 (スキップ)")
            for k, fname in ANDROID_MAP.items():
                if k not in base:
                    continue
                text = base[k]
                write_file(os.path.join(and_dir, fname), text)
            continue
        target_lang = get_claude_lang_code(locale)
        print(f"🌐  {locale} - Claudeターゲット: {target_lang}")
        translations = translate_claude(texts, target_lang, maxs)
        for (k, fname), trans in zip([(k, ANDROID_MAP[k]) for k in keys], translations):
            write_file(os.path.join(and_dir, fname), trans)

    print("✅  All translations written!")


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--csv", required=True, help="Path to CSV (key,value)")
    args = parser.parse_args()
    main(args.csv)
