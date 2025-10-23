# https://gist.github.com/Achneoder/c82174c3e730601554a430f12b0ebfd4

# build the Vue application
FROM node:alpine as dist
ARG API_BASE_URL
ARG GOOGLE_API_KEY
WORKDIR /app
COPY ./ ./
RUN yarn
ENV NODE_ENV=production
ENV VITE_API_BASE_URL=${API_BASE_URL}
ENV VITE_GOOGLE_MAPS_KEY=${GOOGLE_API_KEY}
RUN yarn build

# build the nginx image
FROM nginx:latest
COPY --from=dist /app/dist /usr/share/nginx/html
COPY ./nginx/nginx.conf /etc/nginx/nginx.conf