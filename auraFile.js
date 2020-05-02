const fs = require("fs");

const XORKey = 0x556D6EEC;
const XORKeyUnAligned = 0xB5;

module.exports = class AuraFile
{
    constructor(filePath)
    {
        this.filePtr = 0;
        this.file = fs.readFileSync(filePath);
    }

    readU32()
    {
        const encrypted = this.file.readUInt32LE(this.filePtr);
        this.filePtr += 4;
        return encrypted ^ XORKey;
    }

    readBuffer(size)
    {
        const buffer = this.readRaw(size);
        let i = 0;
        for (; i < buffer.byteLength - (buffer.byteLength % 4); i += 4)
            buffer.writeUInt32LE((buffer.readUInt32LE(i) ^ XORKey) >>> 0, i);
        for (; i < buffer.byteLength; i += 1)
            buffer.writeUInt8((buffer.readUInt8(i) ^ XORKeyUnAligned) >>> 0, i);
        return buffer;
    }

    readString(maxLength)
    {
        const buffer = this.readBuffer(maxLength);
        var stringLength = buffer.findIndex(v => v === 0);
        if (stringLength < 0)
            stringLength = maxLength;
        return buffer.slice(0, stringLength).toString("utf8");
    }

    readRaw(size)
    {
        const buffer = this.file.slice(this.filePtr, this.filePtr + size);
        this.filePtr += size;
        return buffer;
    }

    readFileList()
    {
        let fileCount = this.readU32();
        if (fileCount > 1000)
            throw new Error("invalid archive");
        let fileNames = [];
        for (let i = 0; i < fileCount; i++)
            fileNames.push(this.readString(128));
        return fileNames;
    }

}
