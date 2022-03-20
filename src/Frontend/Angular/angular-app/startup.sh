echo "Starting the app..."
#echo "PALPRIMES_SERVICE_URL: ${PALPRIMES_SERVICE_URL}"
#echo "NOTIFICATIONS_URL: ${NOTIFICATIONS_URL}"
#envsubst < /usr/share/nginx/html/assets/config/app-config.json.template > /usr/share/nginx/html/assets/config/app-config.json
echo "Starting nginx..."
nginx -g 'daemon off;'
