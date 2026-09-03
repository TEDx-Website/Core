import { BaseResponse } from "@/features/auth/types/auth.types";

export interface ProfileAssignments {
  memberOfTrackId: string;
  boardOfTrackId: string;
}

export interface UserProfile {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  bio: string;
  profilePictureUrl: string;
  globalRole: string;
  assignments: ProfileAssignments;
}

export interface ProfilePictureResponse {
  profilePictureUrl: string;
}

export type GetProfileResponse = BaseResponse<UserProfile>;
export type UpdateProfileResponse = BaseResponse<UserProfile>;
export type ProfilePictureUploadResponse = BaseResponse<ProfilePictureResponse>;
export type ChangePasswordResponse = BaseResponse<string>;