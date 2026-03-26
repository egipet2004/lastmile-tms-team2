const USER_FIELDS = `
  id
  firstName
  lastName
  fullName
  email
  phone
  role
  isActive
  isProtected
  depotId
  depotName
  zoneId
  zoneName
  createdAt
  lastModifiedAt
`;

export const USERS_LOOKUPS = `
  query UserManagementLookups {
    userManagementLookups {
      roles {
        value
        label
      }
      depots {
        id
        name
      }
      zones {
        id
        depotId
        name
      }
    }
  }
`;

export const USERS_LIST = `
  query Users(
    $search: String
    $role: UserRole
    $isActive: Boolean
    $depotId: UUID
    $zoneId: UUID
    $skip: Int!
    $take: Int!
  ) {
    users(
      search: $search
      role: $role
      isActive: $isActive
      depotId: $depotId
      zoneId: $zoneId
      skip: $skip
      take: $take
    ) {
      totalCount
      items {
        ${USER_FIELDS}
      }
    }
  }
`;

export const CREATE_USER = `
  mutation CreateUser($input: CreateUserInput!) {
    createUser(input: $input) {
      ${USER_FIELDS}
    }
  }
`;

export const UPDATE_USER = `
  mutation UpdateUser($input: UpdateUserInput!) {
    updateUser(input: $input) {
      ${USER_FIELDS}
    }
  }
`;

export const DEACTIVATE_USER = `
  mutation DeactivateUser($userId: UUID!) {
    deactivateUser(userId: $userId) {
      ${USER_FIELDS}
    }
  }
`;

export const SEND_PASSWORD_RESET_EMAIL = `
  mutation SendPasswordResetEmail($userId: UUID!) {
    sendPasswordResetEmail(userId: $userId) {
      success
      message
    }
  }
`;

export const COMPLETE_PASSWORD_RESET = `
  mutation CompletePasswordReset($input: CompletePasswordResetInput!) {
    completePasswordReset(input: $input) {
      success
      message
    }
  }
`;

export const REQUEST_PASSWORD_RESET = `
  mutation RequestPasswordReset($email: String!) {
    requestPasswordReset(email: $email) {
      success
      message
    }
  }
`;
