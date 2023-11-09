var MessageMissingParameter = "MISSING_PARAM";
var MSG_BAD_PASS = "PASS_ERROR";

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
    let args = e.parameters;
    if (args.pass.toString() !== getPassword())
        return ContentService.createTextOutput(MSG_BAD_PASS);

    if (args.ssid === null)
        return MessageMissingParameter + " \"ssid\"";

    Sheet = SpreadsheetApp.openById(args.ssid.toString());
    return ContentService.createTextOutput(Flow(args));
}

/**
 * @return {string}
 */
function Flow(args) {
    if (args.type === null)
        return MessageMissingParameter + "\"type\"";

    return GetTable(args.type);
}

function GetTable(name) {
    let result = GetSchema(name);
    if (result.status === "error")
        return result.message;

    let schema = result.schema;
    let sheet = Sheet.getSheetByName(name);
    if (sheet.getDataRange().isBlank())
        return "Blank sheet!";

    let values = sheet.getDataRange().getValues();
    let objects = [];

    for (let i = 1; i < values.length; i++) {
        let row = values[i];
        let object = {};
        for (let key in schema) {
            object[key] = {};
            let indexes = schema[key];
            if (indexes.length > 1) {
                let array = [];
                if (row != null) {
                    for (let j = 0; j < indexes.length; j++) {
                        let index = indexes[j];
                        array.push(row[index]);
                    }
                }
                object[key] = array;
            } else {
                if (row == null) {
                    object[key] = null;
                } else {
                    object[key] = row[indexes[0]];
                }
            }
        }
        objects.push(object);
    }

    let json = {
        array: objects
    };

    return JSON.stringify(json);
}

function GetSchema(name) {
    let schema = Sheet.getSheetByName("Schema");
    if (schema.getDataRange().isBlank()) {
        return {
            status: "error",
            message: "Blank Schema!"
        };
    }

    let sheetMap = FindSheetMapInSchema(schema, name);
    if (sheetMap === null) {
        return {
            status: "error",
            message: 'No Schema for "' + name + '"!'
        };
    }

    let jsonObject = ConvertSheetMapToObject(sheetMap);
    return {
        status: "ok",
        schema: jsonObject
    };
}

function FindSheetMapInSchema(schemaSheet, name) {
    let data = schemaSheet.getDataRange().getValues();
    for (let i = 0; i < data.length; i++) {
        let row = data[i];
        if (row[0] == name)
            return row;
    }

    return null;
}

function ConvertSheetMapToObject(sheetMap) {
    let lastIndex = -1;
    let object = {};
    for (let i = 1; i < sheetMap.length; i++) {
        let entry = sheetMap[i];
        if (entry === void 0 || entry === null || entry.length === 0)
            continue;

        lastIndex++;
        let entryData = GetSheetMapEntryData(entry);
        if (entryData.values.length > 0) {
            object[entryData.name] = entryData.values;
            lastIndex = entryData.values[entryData.values.length - 1];
        } else {
            object[entryData.name] = [lastIndex];
        }
    }

    return object;
}

function GetSheetMapEntryData(entry) {
    let dataArray = entry.split(":");
    let entryData = {
        name: dataArray[0],
        values: []
    }

    if (dataArray.length > 1)
        entryData.values = GetColumnIndexes(dataArray[1]);
    return entryData;
}

function GetColumnIndexes(data) {
    let range = data.split("-");
    if (range.length === 2)
        return GetColumnIndexesRange(range);

    let indexes = [];
    let values = data.split(",");
    values.forEach(value => indexes.push(parseInt(value)));
    return indexes;
}

function GetColumnIndexesRange(range) {
    let valueA = parseInt(range[0]);
    let valueB = parseInt(range[1]);
    let min = Math.min(valueA, valueB);
    let max = Math.max(valueA, valueB);

    let indexes = [];
    while (min <= max)
        indexes.push(min++);
    return indexes;
}
