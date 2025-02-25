export const formatDateTime = (input: Date | string):string => {
  const utcDate = typeof input === 'string' ? new Date(input) : input;

  if (isNaN(utcDate.getTime())) {
      throw new Error('Invalid utcDate provided');
  }

  const localTime = new Date(utcDate.getTime() - utcDate.getTimezoneOffset() * 60000);

  const pad = (num: number) => num.toString().padStart(2, '0');

  const hours = pad(localTime.getHours());
  const minutes = pad(localTime.getMinutes());
  const seconds = pad(localTime.getSeconds());
  const day = pad(localTime.getDate());
  const month = pad(localTime.getMonth() + 1); // Months are zero-based
  const year = localTime.getFullYear();

  return `${hours}:${minutes}:${seconds} ${day}/${month}/${year}`;
};