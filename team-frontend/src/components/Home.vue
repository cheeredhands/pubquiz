<template>
  <b-container fluid>
    <b-row>
      <b-col>
        <h1>{{ $t('HOME_WELCOME')}}</h1>
        <b-button
          variant="primary"
          size="lg"
          :to="{name: 'RegisterTeam'}"
        >{{ $t('HOME_REGISTERHERE') }}</b-button>
        <hr />
        <b-link :to="{name: 'QuizMasterLogin'}">{{ $t('HOME_QUIZMASTERLOGIN') }}</b-link>
      </b-col>
    </b-row>
  </b-container>
</template>