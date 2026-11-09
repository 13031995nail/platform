var exec = require('cordova/exec');

var YOLOv5MeterRecognition = {
  detectDigits: function (base64Image, successCallback, errorCallback) {
    exec(successCallback, errorCallback, 'YOLOv5MeterRecognition', 'detectDigits', [base64Image]);
  }
};

module.exports = YOLOv5MeterRecognition;
