import { jsonEndpoint } from '..';
import { useQuery } from '@tanstack/vue-query';
import { useGroupIdStore } from '../group';

const getRanking = (id) => {
  if (!id) {
    return {};
  }
  return jsonEndpoint('groups/' + id + '/ranking');
};

export const useRanking = () => {
  const groupId = useGroupIdStore();
  return useQuery({
    queryKey: ['ranking', groupId],
    queryFn: async () => await getRanking(groupId.value)
  });
};
