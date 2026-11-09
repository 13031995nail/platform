export default class OfflineBackupEntity {
  constructor(modelName, projection, loadCount = 50,
    townPath = undefined, masters = undefined,
    subPartsPaths = undefined, subPartIds = undefined) {
    this.modelName = modelName;
    this.projection = projection;
    this.loadCount = loadCount;
    this.townPath = townPath;
    this.masters = masters;
    this.subPartsPaths = subPartsPaths;
    this.subPartIds = subPartIds;
  }
}