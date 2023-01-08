FROM nginx:1.23-alpine

COPY ./Nginx/* /etc/nginx/
COPY ./run.sh /run.sh

ENV APP_HOST=api
ENV APP_PORT=5000

USER root

RUN apk add --no-cache openssl bash
RUN chmod +x /run.sh

VOLUME /vol/www

CMD ["/run.sh"]