#target photoshop

// Save the current unit preferences (optional)
var startRulerUnits = app.preferences.rulerUnits
var startTypeUnits = app.preferences.typeUnits

// Set units to PIXELS
app.preferences.rulerUnits = Units.PIXELS
app.preferences.typeUnits = TypeUnits.PIXELS

// Use the top-most document
var doc = app.activeDocument; 

// Turn the selection into a work path and give it reference
//doc.selection.makeWorkPath();
var wPath = doc.pathItems[0];

// This will be a string with the final output coordinates
var coords = '';

// Loop through all path points and add their anchor coordinates to the output text
for (var i = 0; i < wPath.subPathItems[0].pathPoints.length; i++)
{
    var x = wPath.subPathItems[0].pathPoints[i].anchor[0];
    var y = wPath.subPathItems[0].pathPoints[i].anchor[1];
    coords += Math.round(x) + "," + Math.round(y);

    if (i < wPath.subPathItems[0].pathPoints.length - 1)
        coords += ";";
}

// Write coords to textfile on the desktop. Thanks krasatos
//var f = File( '~/Desktop/coords.txt' );
//f.open( 'w' );
//f.write( coords );
//f.close();

copyTextToClipboard(coords);

// Reset to previous unit prefs (optional)
app.preferences.rulerUnits = startRulerUnits;
app.preferences.typeUnits = startTypeUnits;

function copyTextToClipboard(txt)
{
    const keyTextData = app.charIDToTypeID('TxtD');
    const ktextToClipboardStr = app.stringIDToTypeID("textToClipboard");

    var textStrDesc = new ActionDescriptor();

    textStrDesc.putString(keyTextData, txt);
    executeAction(ktextToClipboardStr, textStrDesc, DialogModes.NO);
}