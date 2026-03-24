const test = require('node:test');
const assert = require('node:assert/strict');
const { createUserService } = require('../src/services/userService');

// Fake pg Pool-compatible DB for unit tests (no real Postgres needed)
function createFakeDb() {
  const state = { users: [] };

  return {
    pool: {
      async query(sql, params = []) {
        const q = sql.trim().toUpperCase();

        if (q.startsWith('CREATE TABLE')) {
          return { rows: [] };
        }

        if (q.startsWith('INSERT INTO USERS')) {
          const [id, username, email, createdAt] = params;
          const row = { id, username, email, created_at: createdAt };
          state.users.push(row);
          return { rows: [] };
        }

        if (q.includes('FROM USERS WHERE ID = $1')) {
          const row = state.users.find(u => u.id === params[0]);
          return { rows: row ? [row] : [] };
        }

        if (q.startsWith('SELECT ID, USERNAME, EMAIL, CREATED_AT FROM USERS ORDER BY')) {
          // Service orders by created_at DESC
          const rows = [...state.users].sort((a, b) => b.created_at.localeCompare(a.created_at));
          return { rows };
        }

        if (q.startsWith('UPDATE USERS SET USERNAME = $1, EMAIL = $2 WHERE ID = $3 RETURNING ID')) {
          const [username, email, id] = params;
          const idx = state.users.findIndex(u => u.id === id);
          if (idx === -1) {
            return { rows: [] };
          }

          if (state.users.some(u => u.email === email && u.id !== id)) {
            const err = new Error('duplicate key value violates unique constraint');
            err.code = '23505';
            throw err;
          }

          state.users[idx] = { ...state.users[idx], username, email };
          return { rows: [{ id }] };
        }

        if (q.startsWith('DELETE FROM USERS WHERE ID = $1')) {
          const idToDelete = params[0];
          const before = state.users.length;
          // Ensure we compare the same type (string UUIDs)
          state.users = state.users.filter(u => u.id !== idToDelete);
          const rowCount = before - state.users.length;
          return { rowCount };
        }

        throw new Error(`Unhandled SQL in test fake: ${sql}`);
      }
    }
  };
}

function setup() {
  const db = createFakeDb();
  const logger = {
    info() {},
    error() {},
    warn() {}
  };

  return createUserService(db, logger);
}

test('createUser and getUser work', async () => {
  const service = setup();
  const created = await service.createUser({ username: 'ana', email: 'ana@example.com' });

  assert.ok(created.id);
  assert.equal(created.username, 'ana');

  const fetched = await service.getUser(created.id);
  assert.equal(fetched.email, 'ana@example.com');
});

test('createUser throws when input is missing', async () => {
  const service = setup();
  await assert.rejects(() => service.createUser({ username: 'ana' }));
});

test('listUsers returns inserted users', async () => {
  const service = setup();
  await service.createUser({ username: 'ana', email: 'ana@example.com' });
  await service.createUser({ username: 'bor', email: 'bor@example.com' });

  const users = await service.listUsers();
  assert.equal(users.length, 2);
});

test('updateUser updates and returns user', async () => {
  const service = setup();
  const created = await service.createUser({ username: 'ana', email: 'ana@example.com' });

  const updated = await service.updateUser({
    id: created.id,
    username: 'ana2',
    email: 'ana2@example.com'
  });

  assert.equal(updated.username, 'ana2');
  assert.equal(updated.email, 'ana2@example.com');
});