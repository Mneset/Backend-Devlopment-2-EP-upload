export function isInFuture(date: string) {
  return new Date(date).getTime() > Date.now();
}
