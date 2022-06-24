# 按照docker-compose构建docker镜像
docker-compose build

# 根据以下标记重命名镜像
docker tag stargate-webapiserv-web stargate/webapiserv-web:rc1

# 推送到docker hub
docker push stargate/webapiserv-web:rc1
