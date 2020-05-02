const AuraFile = require("./auraFile");

/**
 * @param {AuraFile} file 
 */
function readPSC(file)
{
    let result = "";
    let lineCount = file.readU32();
    for (let i = 0; i < lineCount; i++)
        result += file.readString(file.readU32());
    return result;
};
module.exports =  readPSC;

if (require.main !== module)
    return;

const fs = require("fs");
const file = new AuraFile("014.psc");
const fileNames = file.readFileList();
for (let fileName of fileNames)
    fs.writeFileSync(fileName, readPSC(file));
