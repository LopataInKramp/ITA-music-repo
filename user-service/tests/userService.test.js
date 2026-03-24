const test = require('node:test');
const assert = require('node:assert/strict');
const Database = require('better-sqlite3');
const { createUserService } = require('../src/services/userService');

function setup() {
  const db = new Database(':memory:');
  db.exec(`
    CREATE TABLE users (
      id TEXT PRIMARY KEY,
      username TEXT NOT NULL,
      email TEXT NOT NULL UNIQUE,
      created_at TEXT NOT NULL
    )
  `);

  const logger = {
    info() {},
    error() {}
  };

  return createUserService(db, logger);
}

test('createUser and getUser work', () => {
  const service = setup();
  const created = service.createUser({ username: 'ana', email: 'ana@example.com' });

  assert.ok(created.id);
  assert.equal(created.username, 'ana');

  const fetched = service.getUser(created.id);
  assert.equal(fetched.email, 'ana@example.com');
});

test('createUser throws when input is missing', () => {
  const service = setup();
  assert.throws(() => service.createUser({ username: 'ana' }));
});

test('listUsers returns inserted users', () => {
  const service = setup();
  service.createUser({ username: 'ana', email: 'ana@example.com' });
  service.createUser({ username: 'bor', email: 'bor@example.com' });

  const users = service.listUsers();
  assert.equal(users.length, 2);
});

test('updateUser updates and returns user', () => {
  const service = setup();
  const created = service.createUser({ username: 'ana', email: 'ana@example.com' });

  const updated = service.updateUser({
    id: created.id,
    username: 'ana2',
    email: 'ana2@example.com'
  });

  assert.equal(updated.username, 'ana2');
  assert.equal(updated.email, 'ana2@example.com');
});

test('deleteUser removes existing user', () => {
  const service = setup();
  const created = service.createUser({ username: 'ana', email: 'ana@example.com' });

  const removed = service.deleteUser(created.id);
  assert.equal(removed, true);
  assert.equal(service.getUser(created.id), null);
});

