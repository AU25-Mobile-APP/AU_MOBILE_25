
// Handles extended object params format
function replaceIndexedObjectPlaceholders(placeholder, values) {
  return values.params && placeholder in values.params ? values.params[placeholder] : `{${placeholder}}`;
}

function replaceNonIndexedPlaceholders(placeholder, values) {
  return placeholder in values ? values[placeholder] : replaceIndexedObjectPlaceholders(placeholder, values);
}

function replaceIndexedPlaceholders(index, placeholder, values) {
  return index in values.params ? values.params[index] : `${index}:{${placeholder}}`;
}

function replaceCb(values, param, placeholder) {
  if (!param || !values.params) {
    return replaceNonIndexedPlaceholders(placeholder, values);
  }

  // Handles extended object params format
  if (!Array.isArray(values.params)) {
    return replaceIndexedObjectPlaceholders(placeholder, values);
  }

  // Extended Params exit in the format of `paramIndex:{paramName}` where the index is optional
  const paramIndex = Number(param.replace(':', ''));

  return replaceIndexedPlaceholders(paramIndex, placeholder, values);
}

/**
 * Replaces placeholder values in a string with their actual values
 */
export function interpolate(template, values) {
  const captureRegex = /(?<param>\d:)?\{(?<placeholder>[^}]*)\}/g;

  return template.replace(captureRegex, (_, param, placeholder) => replaceCb(values, param, placeholder));
}
