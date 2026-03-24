const path = require('path');
const grpc = require('@grpc/grpc-js');
const protoLoader = require('@grpc/proto-loader');
const logger = require('./logger');
const { createDb } = require('./db/sqlite');
const { createUserService } = require('./services/userService');

const PROTO_PATH = path.join(__dirname, '..', 'proto', 'user.proto');
const packageDefinition = protoLoader.loadSync(PROTO_PATH, {
  keepCase: true,
  longs: String,
  enums: String,
  defaults: true,
  oneofs: true
});

const userProto = grpc.loadPackageDefinition(packageDefinition).user;

function grpcError(code, message) {
  const err = new Error(message);
  err.code = code;
  return err;
}

function createHandlers(userService) {
  return {
    async CreateUser(call, callback) {
      try {
        const user = await userService.createUser(call.request);
        callback(null, { user });
      } catch (error) {
        logger.error({ err: error }, 'CreateUser failed');
        if (String(error.message).includes('UNIQUE')) {
          return callback(grpcError(grpc.status.ALREADY_EXISTS, 'email already exists'));
        }
        return callback(grpcError(grpc.status.INVALID_ARGUMENT, error.message));
      }
    },

    async GetUser(call, callback) {
      try {
        const user = await userService.getUser(call.request.id);
        if (!user) {
          return callback(grpcError(grpc.status.NOT_FOUND, 'user not found'));
        }
        return callback(null, { user });
      } catch (error) {
        logger.error({ err: error }, 'GetUser failed');
        return callback(grpcError(grpc.status.INVALID_ARGUMENT, error.message));
      }
    },

    async ListUsers(_call, callback) {
      try {
        const users = await userService.listUsers();
        callback(null, { users });
      } catch (error) {
        logger.error({ err: error }, 'ListUsers failed');
        callback(grpcError(grpc.status.INTERNAL, 'internal error'));
      }
    },

    async UpdateUser(call, callback) {
      try {
        const user = await userService.updateUser(call.request);
        if (!user) {
          return callback(grpcError(grpc.status.NOT_FOUND, 'user not found'));
        }
        return callback(null, { user });
      } catch (error) {
        logger.error({ err: error }, 'UpdateUser failed');
        if (String(error.message).includes('UNIQUE')) {
          return callback(grpcError(grpc.status.ALREADY_EXISTS, 'email already exists'));
        }
        return callback(grpcError(grpc.status.INVALID_ARGUMENT, error.message));
      }
    },

    async DeleteUser(call, callback) {
      try {
        const success = await userService.deleteUser(call.request.id);
        return callback(null, { success });
      } catch (error) {
        logger.error({ err: error }, 'DeleteUser failed');
        return callback(grpcError(grpc.status.INVALID_ARGUMENT, error.message));
      }
    }
  };
}


async function start() {
  const db = createDb(logger);
  await db.init();

  const userService = createUserService(db, logger);
  const handlers = createHandlers(userService);

  const server = new grpc.Server();
  server.addService(userProto.UserService.service, handlers);

  const host = process.env.GRPC_HOST || '0.0.0.0';
  const port = process.env.GRPC_PORT || '50051';
  const address = `${host}:${port}`;

  server.bindAsync(address, grpc.ServerCredentials.createInsecure(), (err) => {
    if (err) {
      logger.error({ err }, 'Failed to start gRPC server');
      process.exit(1);
    }

    server.start();
    logger.info({ address }, 'gRPC user-service started');
  });
}

if (require.main === module) {
  start().catch((err) => {
    logger.error({ err }, 'Failed to start service');
    process.exit(1);
  });
}

module.exports = { start, createHandlers };

