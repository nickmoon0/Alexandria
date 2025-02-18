export const formatDateTime = (input: Date | string):string => {
  const date = typeof input === "string" ? new Date(input) : input;

  if (isNaN(date.getTime())) {
      throw new Error("Invalid date provided");
  }

  const pad = (num: number) => num.toString().padStart(2, "0");

  const hours = pad(date.getHours());
  const minutes = pad(date.getMinutes());
  const seconds = pad(date.getSeconds());
  const day = pad(date.getDate());
  const month = pad(date.getMonth() + 1); // Months are zero-based
  const year = date.getFullYear();

  return `${hours}:${minutes}:${seconds} ${day}/${month}/${year}`;
};