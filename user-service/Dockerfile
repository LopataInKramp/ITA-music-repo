FROM node:20-alpine

WORKDIR /app

COPY package*.json ./
RUN npm ci --omit=dev

COPY . .

ENV NODE_ENV=production
ENV GRPC_PORT=50051

EXPOSE 50051

CMD ["npm", "start"]

