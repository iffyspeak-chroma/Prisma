const { exec } = require('child_process');
const fs = require('fs');
const path = require('path');
const readline = require('readline');

const ignorePatterns = [
  /^\.idea\//,     // anything in .idea folder
  /^\.gitignore$/, // the .gitignore file itself
];

function shouldIgnore(file) {
  return ignorePatterns.some((pattern) => pattern.test(file));
}

function countLines(filePath) {
  return new Promise((resolve, reject) => {
    let lineCount = 0;

    const stream = fs.createReadStream(filePath, { encoding: 'utf-8' });
    const rl = readline.createInterface({ input: stream });

    rl.on('line', (line) => {
      if (line.trim() !== '') lineCount++;
    });

    rl.on('close', () => resolve(lineCount));
    rl.on('error', reject);
  });
}

exec('git ls-files', async (err, stdout) => {
  if (err) {
    console.error('Error running git ls-files:', err);
    return;
  }

  const files = stdout
    .split('\n')
    .filter(Boolean)
    .filter((file) => !shouldIgnore(file));

  const results = [];
  let totalLines = 0;

  for (const file of files) {
    try {
      const filePath = path.resolve(file);
      const lines = await countLines(filePath);
      totalLines += lines;

      results.push({ file, lines });
      console.log(`${file}: ${lines} lines`);
    } catch (e) {
      console.error(`Failed to read ${file}:`, e.message);
    }
  }

  const largest = [...results]
    .sort((a, b) => b.lines - a.lines)
    .slice(0, 3);

  const smallest = [...results]
    .filter(r => r.lines > 0)
    .sort((a, b) => a.lines - b.lines)
    .slice(0, 3);

  console.log('\nFattest line counts in repo:');
  largest.forEach(r => console.log(`${r.file}: ${r.lines}`));

  console.log('\nThinnest line counts in repo:');
  smallest.forEach(r => console.log(`${r.file}: ${r.lines}`));
  
  console.log(`\nTotal lines of code: ${totalLines}`);
});