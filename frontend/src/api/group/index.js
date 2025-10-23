import { useSessionStorage } from '@vueuse/core';
import { ApiError, jsonEndpoint, jsonPostEndpoint } from '..';
import { useQuery } from '@tanstack/vue-query';
import router from '@/router';

const getGroup = async (id) => {
  if (!id) {
    return null;
  }
  try {
    return await jsonEndpoint('groups/' + id);
  } catch (error) {
    handleGroupRequestError(error);
    return null;
  }
};

const handleGroupRequestError = (error) => {
  if (!(error instanceof ApiError)) {
    throw error;
  }
  if (error.response.status === 404) {
    const groupStore = useGroupIdStore();
    groupStore.value = '';
    router.push({ name: 'CreateGroup' });
  }
};

export const useGroupIdStore = () => {
  return useSessionStorage('sessionId', '');
};

export const useGroup = () => {
  const groupId = useGroupIdStore();
  return useQuery({
    queryKey: ['groups', groupId],
    queryFn: async () => await getGroup(groupId.value)
  });
};

export const createGroup = (data) => {
  return jsonPostEndpoint('groups', data).then((res) => {
    const groupStore = useGroupIdStore();
    groupStore.value = res;
    return res;
  });
};

export const joinGroup = (id) => {
  return jsonPostEndpoint(`groups/${id}/join`).then((res) => {
    const groupStore = useGroupIdStore();
    groupStore.value = res;
    return res;
  });
};

export const leaveGroup = () => {
  const store = useGroupIdStore();
  if (!store.value) {
    return null;
  }
  return jsonPostEndpoint(`groups/${store.value}/leave`).then((res) => {
    const groupStore = useGroupIdStore();
    groupStore.value = '';
    return res;
  });
};
