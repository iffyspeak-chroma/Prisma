const { exec } = require('child_process');
const fs = require('fs');
const path = require('path');
const readline = require('readline');

function countLines(filePath) {
  return new Promise((resolve, reject) => {
    let lineCount = 0;

    const stream = fs.createReadStream(filePath, { encoding: 'utf-8' });
    const rl = readline.createInterface({ input: stream });

    rl.on('line', (line) => {
      if (line.trim() !== '') lineCount++; // ignore empty lines
    });

    rl.on('close', () => resolve(lineCount));
    rl.on('error', reject);
  });
}

// Run git ls-files
exec('git ls-files', async (err, stdout, stderr) => {
  if (err) {
    console.error('Error running git ls-files:', err);
    return;
  }

  const files = stdout.split('\n').filter(Boolean);
  let totalLines = 0;

  for (const file of files) {
    try {
      const filePath = path.resolve(file);
      const lines = await countLines(filePath);
      totalLines += lines;
      console.log(`${file}: ${lines} lines`);
    } catch (e) {
      console.error(`Failed to read ${file}:`, e.message);
    }
  }

  console.log(`\nTotal lines of code: ${totalLines}`);
});