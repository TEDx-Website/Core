import { create } from "zustand";

interface UserData {
  id?: string;
  firstName?: string;
  lastName?: string;
  profilePictureUrl?: string;
  email?: string;
  globalRole?: string;
}

interface UserStore {
  user: UserData | null;
  setUser: (user: UserData | null) => void;
  updateUser: (data: Partial<UserData>) => void;
}

export const useUserStore = create<UserStore>((set) => ({
  user: null,
  
  setUser: (user) => set({ user }),
  
  updateUser: (data) =>
    set((state) => ({
      user: state.user ? { ...state.user, ...data } : null,
    })),
}));