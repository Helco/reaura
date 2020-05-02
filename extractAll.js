const fs = require("fs");
const path = require("path");
const AuraFile = require("./auraFile");
const ReadPSC = require("./psc");

const directory = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Aura Fate of the Ages\\Scenes\\";

function extractArchive(scene, ext, archivePath)
{
    const outDir = path.join("out", scene, scene + ext);
    fs.mkdirSync(outDir, { recursive: true });
    const file = new AuraFile(archivePath);
    const fileNames = file.readFileList();
    for (let fileName of fileNames)
    {
        const size = file.readU32();
        const buffer = file.readRaw(size);
        fs.writeFileSync(path.join(outDir, fileName), buffer);
    }
}

function extractPSC(scene, archivePath)
{
    const outDir = path.join("out", scene, scene + ".psc");
    fs.mkdirSync(outDir, { recursive: true });
    const file = new AuraFile(archivePath);
    const fileNames = file.readFileList();
    for (let fileName of fileNames)
    {
        fs.writeFileSync(path.join(outDir, fileName + ".txt"), ReadPSC(file));
    }
}

function extractScene(scene)
{
    const ARCHIVE_EXTS = [
        ".psp",
        ".pvd",
        ".psp"
    ];
    const sceneDir = path.join(directory, scene);
    for (let ext of ARCHIVE_EXTS)
    {
        const archivePath = path.join(sceneDir, scene + ext);
        if (fs.existsSync(archivePath))
            extractArchive(scene, ext, archivePath);
    }
    const pscArchivePath = path.join(sceneDir, scene + ".psc");
    if (fs.existsSync(pscArchivePath))
        extractPSC(scene, pscArchivePath);
}

const scenes = fs.readdirSync(directory);
for (let scene of scenes)
{
    extractScene(scene);
    if (global.gc)
        global.gc();
}
