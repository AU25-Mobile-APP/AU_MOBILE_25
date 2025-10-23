import { ref } from "vue";

export const swipingLeft = ref(false);
export const swipingRight = ref(false);

const isDragging = ref(false);

export const onDragStart = (item, event) => {
  event.dataTransfer.setData('text/plain', JSON.stringify(item));
  isDragging.value = true;
};

export const onDragEnd = () => {
  isDragging.value = false;
};

export const onDropRight = (_, next) => {
  if (isDragging.value) {
    next();
  }
  isDragging.value = false;
  swipingRight.value = false;
};

export const onDropLeft = (_, next) => {
  if (isDragging.value) {
    next();
  }
  isDragging.value = false;
  swipingLeft.value = false;
};

export const onDragEnterRight = () => {
  if (isDragging.value) {
    swipingRight.value = true;
  }
};

export const onDragLeaveRight = () => {
  swipingRight.value = false;
};

export const onDragEnterLeft = () => {
  if (isDragging.value) {
    swipingLeft.value = true;
  }
};

export const onDragLeaveLeft = () => {
  swipingLeft.value = false;
};
