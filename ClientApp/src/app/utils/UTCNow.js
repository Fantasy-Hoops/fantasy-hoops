export const UTCNow = () => {
  const now = new Date();
  const utc_now = new Date(
    now.getUTCFullYear(),
    now.getUTCMonth(),
    now.getUTCDate(),
    now.getUTCHours(),
    now.getUTCMinutes(),
    now.getUTCSeconds(),
    now.getUTCMilliseconds()
  );
  const UTCNow = Date.prototype.getTime.bind(utc_now);
  return {
    Time: utc_now,
    Function: UTCNow
  };
};
