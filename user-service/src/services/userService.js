const { randomUUID } = require('crypto');

function createUserService(db, logger) {
  function toUser(row) {
    return {
      id: row.id,
      username: row.username,
      email: row.email,
      createdAt: row.created_at
    };
  }

  async function createUser({ username, email }) {
    if (!username || !email) {
      throw new Error('username and email are required');
    }

    const id = randomUUID();
    const createdAt = new Date().toISOString();

    await db.pool.query(
      'INSERT INTO users (id, username, email, created_at) VALUES ($1, $2, $3, $4)',
      [id, username, email, createdAt]
    );

    logger.info({ id, username, email }, 'User created');
    return { id, username, email, createdAt };
  }

  async function getUser(id) {
    if (!id) {
      throw new Error('id is required');
    }

    const result = await db.pool.query(
      'SELECT id, username, email, created_at FROM users WHERE id = $1',
      [id]
    );

    if (result.rows.length === 0) {
      return null;
    }

    return toUser(result.rows[0]);
  }

  async function listUsers() {
    const result = await db.pool.query(
      'SELECT id, username, email, created_at FROM users ORDER BY created_at DESC'
    );
    return result.rows.map(toUser);
  }

  async function updateUser({ id, username, email }) {
    if (!id || !username || !email) {
      throw new Error('id, username and email are required');
    }

    const result = await db.pool.query(
      'UPDATE users SET username = $1, email = $2 WHERE id = $3 RETURNING id',
      [username, email, id]
    );

    if (result.rows.length === 0) {
      return null;
    }

    logger.info({ id, username, email }, 'User updated');
    return getUser(id);
  }

  async function deleteUser(id) {
    if (!id) {
      throw new Error('id is required');
    }

    const result = await db.pool.query(
      'DELETE FROM users WHERE id = $1',
      [id]
    );

    const success = result.rowCount > 0;

    if (success) {
      logger.info({ id }, 'User deleted');
    }

    return success;
  }

  return {
    createUser,
    getUser,
    listUsers,
    updateUser,
    deleteUser
  };
}

module.exports = { createUserService };

