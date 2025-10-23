import { createRouter, createWebHistory } from 'vue-router';
import Default from '@/layout/Default.vue';
import CreateGroupPage from '@/pages/groups/CreateGroupPage.vue';
import Home from '@/pages/Home.vue';
import ViewGroupPage from '@/pages/groups/ViewGroupPage.vue';
import RankingTable from '@/pages/groups/RankingTable.vue';
import { useGroupIdStore } from '@/api/group';

const routes = [
  {
    path: '/',
    component: Default,
    children: [
      {
        path: '',
        name: 'Home',
        component: Home,
        beforeEnter: () => {
          const groupId = useGroupIdStore();

          if (groupId.value) {
            return { name: 'ViewGroup', params: { id: groupId.value } };
          }
        },
      },
      {
        path: 'group',
        name: 'Group',
        children: [
          {
            path: '',
            name: 'CreateGroup',
            component: CreateGroupPage,
          },
          {
            path: 'view/:id',
            name: 'ViewGroup',
            component: ViewGroupPage,
            props: true,
            beforeEnter: (to) => {
              const groupId = useGroupIdStore();

              if (!groupId.value) {
                return { name: 'Home' };
              }

              if (groupId.value !== to.params.id) {
                return { name: 'ViewGroup', params: { id: groupId.value } };
              }
            },
          },
          {
            path: 'ranking/:id',
            name: 'GroupRanking',
            component: RankingTable,
            props: true,
            beforeEnter: (to) => {
              const groupId = useGroupIdStore();

              if (!groupId.value) {
                return { name: 'Home' };
              }

              if (groupId.value !== to.params.id) {
                return { name: 'GroupRanking', params: { id: groupId.value } };
              }
            },
          }
        ]
      },
      {
        path: ':pathMatch(.*)*',
        redirect: (to) => ({
          name: '404', query: {
            toPath: to.fullPath,
          },
        }),
      },
    ],
  },
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});


export default router;
