const DEV_BASE_URL = 'http://localhost:5072/api/v1/';
// TODO: Add PROD deployed URL
const PROD_BASE_URL = '';

export class ApiError extends Error {
  constructor(message, response) {
    super(message);
    this.response = response;
  }
}

export const getApiBase = () => {
  if (import.meta.env.DEV) {
    return DEV_BASE_URL;
  }
  if (import.meta.env.VITE_API_BASE_URL) {
    return import.meta.env.VITE_API_BASE_URL;
  }

  return PROD_BASE_URL;
};

const handleApiError = async (response) => {
  if (!response.ok) {
    try {
      const problem = await response.json();
      if (problem?.title) {
        throw new ApiError(problem.title, response);
      }
    } catch (error) {
      throw new ApiError('Server Error', response);
    }
  }
  return response;
};

export const jsonEndpoint = (endpoint, params = {}) => {
  const url = getApiBase() + endpoint;
  params.headers = {
    ...params.headers || {},
    'Content-Type': 'application/json'
  };
  params.credentials = 'include';
  return fetch(url, params).then(handleApiError).then(async (data) => {
    const response = await data.text();
    if (response === '') {
      return {};
    }
    return JSON.parse(response);
  });
};

export const jsonPostEndpoint = (endpoint, body = {}) => {
  return jsonEndpoint(endpoint, {
    method: 'POST',
    body: JSON.stringify(body)
  });
};

export const jsonPatchEndpoint = (endpoint, body = {}) => {
  return jsonEndpoint(endpoint, {
    method: 'PATCH',
    body: JSON.stringify(body)
  });
};

export const jsonPutEndpoint = (endpoint, body = {}) => {
  return jsonEndpoint(endpoint, {
    method: 'PUT',
    body: JSON.stringify(body)
  });
};
