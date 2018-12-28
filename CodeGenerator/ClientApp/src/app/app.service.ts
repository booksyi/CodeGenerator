export const buildQueryParams = (opt): string =>
  Object.keys(opt)
    .filter(key => {
      if (Array.isArray(opt[key] && opt[key].length === 0)) {
        return false;
      }
      return opt[key] != null;
    })
    .reduce((params, key) => {
      // return [params, `${key}=${opt[key]}`].join('&');
      if (opt[key].toLocaleDateString && typeof opt[key].toLocaleDateString === 'function') {
        const dateString = opt[key].toLocaleDateString();
        return params + `${key}=${dateString}` + '&';
      }
      if (Array.isArray(opt[key])) {
        return (
          params +
          (<Array<any>>opt[key]).map(k => `${key}=${k}`).join('&') +
          '&'
        );
      }

      return params + `${key}=${opt[key]}` + '&';
    }, '?');

export interface List<T> {
  data: T[];
}

export interface IResponse {
  value: any | string;
}
