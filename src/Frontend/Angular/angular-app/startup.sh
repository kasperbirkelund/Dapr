echo "Starting the app..."
echo "RESOURCES_API_ROOT_URL: ${RESOURCES_API_ROOT_URL}"
envsubst < /usr/share/nginx/html/assets/config/app-config.json.template > /usr/share/nginx/html/assets/config/app-config.json
echo "Starting nginx..."
nginx -g 'daemon off;'
