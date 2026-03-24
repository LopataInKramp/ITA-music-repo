const { Pool } = require('pg');

function createDb(logger) {
  const connectionString =
      process.env.DATABASE_URL ||
      'postgres://user:userpass@user-db:5432/userdb';

  const pool = new Pool({ connectionString });

  async function retryConnection(maxRetries = 10, delayMs = 1000) {
    for (let attempt = 1; attempt <= maxRetries; attempt++) {
      try {
        const client = await pool.connect();
        client.release();
        logger?.info({ attempt }, 'Database connection successful');
        return;
      } catch (error) {
        logger?.warn({ attempt, maxRetries, error: error.message }, 'Database connection failed, retrying...');
        if (attempt === maxRetries) throw error;
        await new Promise(resolve => setTimeout(resolve, delayMs * attempt));
      }
    }
  }

  async function init() {
    await retryConnection();
    await pool.query(`
      CREATE TABLE IF NOT EXISTS users (
        id TEXT PRIMARY KEY,
        username TEXT NOT NULL,
        email TEXT NOT NULL UNIQUE,
        created_at TEXT NOT NULL
      )
    `);
    logger?.info('Users table created/verified');
  }

  return {
    init,
    pool,
    async close() {
      await pool.end();
    }
  };
}

module.exports = { createDb };
