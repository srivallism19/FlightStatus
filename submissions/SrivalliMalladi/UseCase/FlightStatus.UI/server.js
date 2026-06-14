const express = require('express');
const path = require('path');

const app = express();
const distFolder = path.join(__dirname, 'dist', 'FlightStatus.UI', 'browser');

app.use(express.static(distFolder));

app.get('*', (req, res) => {
  res.sendFile(path.join(distFolder, 'index.html'));
});

const port = process.env.PORT || 8080;
app.listen(port, () => {
  console.log(`Server running on port ${port}`);
});
