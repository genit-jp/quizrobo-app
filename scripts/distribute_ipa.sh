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

echo "*************************prod*************************"
sh build_ios_prod.sh
sh archive_xcproject.sh prod
sh build_ipa.sh prod
mkdir -p ./distribution/jp.genit.kidsquiz
cp ../build/prod/export/ProductName.ipa ./distribution/jp.genit.kidsquiz/app.ipa
sh make_distribution_manifest.sh jp.genit.kidsquiz

aws s3 sync ./distribution/ s3://genit.dev/apps/