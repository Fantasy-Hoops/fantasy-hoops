export const handleErrors = async (e) => {
  if (e.status !== 200) {
    let message = await e.statusText;
    if (message === '')
      throw Error(e.status + " " + e.statusText);
    throw Error(e.status + " " + message);
  }
  return e;
}