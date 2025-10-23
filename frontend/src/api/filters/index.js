import { jsonEndpoint, jsonPatchEndpoint } from '..';
import { useQuery } from '@tanstack/vue-query';
import { useGroup, useGroupIdStore } from '../group';

const getFilters = (id) => {
  if (!id) {
    return {};
  }
  return jsonEndpoint('filters/' + id);
};

const getDefaultFilters = () => {
  return jsonEndpoint('defaultfilters');
};

export const useFilters = () => {
  const group = useGroup();
  return useQuery({
    queryKey: ['filters', group],
    queryFn: async () => await getFilters(group.value)
  });
};

export const useDefaultFilters = () => {
  return useQuery({
    queryKey: ['defaultFilters'],
    queryFn: async () => await getDefaultFilters()
  });
};

export const updateFilters = async (newFilters) => {
  const groupId = useGroupIdStore();
  return await jsonPatchEndpoint(`filters/${groupId.value}`, newFilters);

};
