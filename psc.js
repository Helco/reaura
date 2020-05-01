const fs = require("fs");

const XORKey = 0x556D6EEC;
const XORKeyUnAligned = 0xB5;

let filePtr = 0;
const file = fs.readFileSync("Puzzle_Gong.psc");

function readU32()
{
    const encrypted = file.readUInt32LE(filePtr);
    filePtr += 4;
    return encrypted ^ XORKey;
}

function readBuffer(size)
{
    const buffer = file.slice(filePtr, filePtr + size);
    filePtr += size;
    let i = 0;
    for (; i < buffer.byteLength - (buffer.byteLength % 4); i += 4)
        buffer.writeUInt32LE((buffer.readUInt32LE(i) ^ XORKey) >>> 0, i);
    for (; i < buffer.byteLength; i += 1)
        buffer.writeUInt8((buffer.readUInt8(i) ^ XORKeyUnAligned) >>> 0, i);
    return buffer;
}

function readString(maxLength)
{
    const buffer = readBuffer(maxLength);
    var stringLength = buffer.findIndex(v => v === 0);
    if (stringLength < 0)
        stringLength = maxLength;
    return buffer.slice(0, stringLength).toString("utf8");
}

// read filenames
let fileCount = readU32();
let fileNames = [];
for (let i = 0; i < fileCount; i++)
    fileNames.push(readString(128));

// read files
let files = new Map();
for (let fileName of fileNames)
{
    let lineCount = readU32();
    let lines = [];
    for (let i = 0; i < lineCount; i++)
    {
        let lineLength = readU32();
        lines.push(readString(lineLength));
    }
    files.set(fileName, lines);
}

// write files
for (let pair of files)
{
    fs.writeFileSync(pair[0] + ".txt", pair[1].join(""));
}
