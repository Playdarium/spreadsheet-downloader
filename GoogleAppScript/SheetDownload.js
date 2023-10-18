var MessageMissingParameter = "MISSING_PARAM";
var MSG_BAD_PASS = "PASS_ERROR";

// Here you can set custom password for access
var PASSWORD = "<custom-password>";

var Sheet;

// ************** Entry Point **************

function doPost(e) {
    return Entry(e);
}

function doGet(e) {
    return Entry(e);
}

// ************** Initialization functions **************

function Entry(e) {
    if (e.parameter.pass !== PASSWORD)
        return ContentService.createTextOutput(MSG_BAD_PASS);

    if (e.parameters.ssid === null)
        return MessageMissingParameter + " \"ssid\"";

    Sheet = SpreadsheetApp.openById(e.parameters.ssid.toString());
    return ContentService.createTextOutput(Flow(e));
}

/**
 * @return {string}
 */
function Flow(arg) {
    if (args.type === null)
        return MessageMissingParameter + "\"type\"";

    return GetTable(args.type);
}

function GetTable(name) {
    let sheet = Sheet.getSheetByName(name);
    if (sheet == null)
        return "No sheet with name: " + name;

    if (sheet.getDataRange().isBlank())
        return "Blank sheet!";

    let values = sheet.getDataRange().getValues();
    let headers = values[0];
    let objects = [];

    for (let i = 1; i < values.length; i++) {
        let row = values[i];
        let object = {};

        for (let j = 0; j < row.length; j++) {
            let header = headers[j];
            if (isNullOrEmptyOrBlank(header) || !isStringSuitable(header))
                continue;

            let key = toCamelCase(header);
            object[key] = row[j];
        }

        objects.push(object);
    }

    return JSON.stringify(objects);
}

function toCamelCase(inputString) {
    // Replace spaces, hyphens, and underscores with a single space
    inputString = inputString.replace(/[-_\s]+/g, ' ');

    // Split the string into words
    const words = inputString.split(' ');

    // Capitalize the first letter of each word and convert the rest to lowercase
    const camelCaseWords = words.map((word, index) => {
        if (index === 0) {
            return word.toLowerCase();
        } else {
            return word.charAt(0).toUpperCase() + word.slice(1).toLowerCase();
        }
    });

    // Join the words to form the camel case string
    return camelCaseWords.join('');
}

function isStringSuitable(inputString) {
    // Define a regular expression pattern to match alphanumeric characters and spaces
    var pattern = /^[a-zA-Z0-9\s]+$/;

    // Use the test() method to check if the inputString matches the pattern
    return pattern.test(inputString);
}

function isNullOrEmptyOrBlank(str) {
    if (str == null || str.trim() === '')
        return true;

    return false;
}
