FROM node:16-alpine3.11 AS os
ENV DEBIAN_FRONTEND=noninteractive

#RUN apk add --update python make g++\
#   && rm -rf /var/cache/apk/*
#RUN apk add --no-cache chromium --repository=http://dl-cdn.alpinelinux.org/alpine/v3.11/main
#ENV CHROME_BIN=/usr/bin/chromium-browser   

FROM os as builder
COPY package.json package-lock.json ./
RUN npm ci && mkdir /ng-app && mv ./node_modules ./ng-app
WORKDIR /ng-app
COPY src ./src/
COPY *.* ./
RUN npm run ng build -- --prod --output-path=dist

FROM nginx:1.20.0-alpine as host
COPY nginx.conf /etc/nginx/nginx.conf
RUN rm -rf /usr/share/nginx/html/*
COPY --from=builder /ng-app/dist /usr/share/nginx/html

FROM host as run
ADD startup.sh /startup.sh
RUN ["chmod", "+x", "/startup.sh"]
RUN sed -i 's/\r//g' "/startup.sh"
ENTRYPOINT /startup.sh