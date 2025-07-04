#!/bin/sh
cd `dirname $0`

# sh build_ios_dev.sh

echo "*************************dev*************************"
sh build_ios_dev.sh
sh archive_xcproject.sh dev
sh build_ipa.sh dev
mkdir -p ./distribution/jp.genit.kidsquiz.dev
cp ../build/dev/export/ProductName.ipa ./distribution/jp.genit.kidsquiz.dev/app.ipa
sh make_distribution_manifest.sh jp.genit.kidsquiz.dev


aws s3 sync ./distribution/jp.genit.kidsquiz.dev s3://genit.dev/apps/jp.genit.kidsquiz.dev